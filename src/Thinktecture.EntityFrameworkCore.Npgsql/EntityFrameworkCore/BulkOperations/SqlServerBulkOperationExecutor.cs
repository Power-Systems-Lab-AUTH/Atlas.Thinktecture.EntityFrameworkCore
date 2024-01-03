using System.Diagnostics;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Thinktecture.EntityFrameworkCore.Data;
using Thinktecture.Internal;

namespace Thinktecture.EntityFrameworkCore.BulkOperations;

/// <summary>
/// Executes bulk operations.
/// </summary>
public sealed class NpgsqlBulkOperationExecutor
   : IBulkInsertExecutor, ITruncateTableExecutor
{
   private readonly DbContext _ctx;
   private readonly IDiagnosticsLogger<NpgsqlDbLoggerCategory.BulkOperation> _logger;
   private readonly ISqlGenerationHelper _sqlGenerationHelper;
   private readonly ObjectPool<StringBuilder> _stringBuilderPool;

   private static class EventIds
   {
      public static readonly EventId Inserting = 0;
      public static readonly EventId Inserted = 1;
   }

   /// <summary>
   /// Initializes new instance of <see cref="NpgsqlBulkOperationExecutor"/>.
   /// </summary>
   /// <param name="ctx">Current database context.</param>
   /// <param name="logger">Logger.</param>
   /// <param name="sqlGenerationHelper">SQL generation helper.</param>
   /// <param name="stringBuilderPool">String builder pool.</param>
   public NpgsqlBulkOperationExecutor(
      ICurrentDbContext ctx,
      IDiagnosticsLogger<NpgsqlDbLoggerCategory.BulkOperation> logger,
      ISqlGenerationHelper sqlGenerationHelper,
      ObjectPool<StringBuilder> stringBuilderPool)
   {
      ArgumentNullException.ThrowIfNull(ctx);

      _ctx = ctx.Context;
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _sqlGenerationHelper = sqlGenerationHelper ?? throw new ArgumentNullException(nameof(sqlGenerationHelper));
      _stringBuilderPool = stringBuilderPool ?? throw new ArgumentNullException(nameof(stringBuilderPool));
   }

   /// <inheritdoc />
   IBulkInsertOptions IBulkInsertExecutor.CreateOptions(IEntityPropertiesProvider? propertiesToInsert)
   {
      return new NpgsqlBulkInsertOptions { PropertiesToInsert = propertiesToInsert };
   }
   /// <inheritdoc />
   public Task BulkInsertAsync<T>(
      IEnumerable<T> entities,
      IBulkInsertOptions options,
      CancellationToken cancellationToken = default)
      where T : class
   {
      var entityType = _ctx.Model.GetEntityType(typeof(T));
      var tableName = entityType.GetTableName() ?? throw new InvalidOperationException($"The entity '{entityType.Name}' has no table name.");

      return BulkInsertAsync(entityType, entities, entityType.GetSchema(), tableName, options, NpgsqlBulkOperationContextFactoryForEntities.Instance, cancellationToken);
   }

   private async Task BulkInsertAsync<T>(
      IEntityType entityType,
      IEnumerable<T> entitiesOrValues,
      string? schema,
      string tableName,
      IBulkInsertOptions options,
      INpgsqlBulkOperationContextFactory bulkOperationContextFactory,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(entitiesOrValues);
      ArgumentNullException.ThrowIfNull(tableName);
      ArgumentNullException.ThrowIfNull(options);

      if (options is not NpgsqlBulkInsertOptions sqlServerOptions)
         sqlServerOptions = new NpgsqlBulkInsertOptions(options);

      var properties = sqlServerOptions.PropertiesToInsert.DeterminePropertiesForInsert(entityType, null);
      properties.EnsureNoSeparateOwnedTypesInsideCollectionOwnedType();

      var ctx = bulkOperationContextFactory.CreateForBulkInsert(_ctx, sqlServerOptions, properties);

      await BulkInsertAsync(entitiesOrValues, schema, tableName, ctx, cancellationToken);
   }

   private async Task BulkInsertAsync<T>(
      IEnumerable<T> entitiesOrValues,
      string? schema,
      string tableName,
      INpgsqlBulkOperationContext ctx,
      CancellationToken cancellationToken)
   {
      using var reader = ctx.CreateReader(entitiesOrValues);
      using var bulkCopy = CreateSqlBulkCopy(ctx.Connection, ctx.Transaction, schema, tableName, ctx.Options);

      var columns = SetColumnMappings(bulkCopy, reader);

      await _ctx.Database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

      try
      {
         LogInserting(ctx.Options.SqlBulkCopyOptions, bulkCopy, columns);
         var stopwatch = Stopwatch.StartNew();

         await bulkCopy.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);

         LogInserted(ctx.Options.SqlBulkCopyOptions, stopwatch.Elapsed, bulkCopy, columns);

         if (ctx.HasExternalProperties)
         {
            var readEntities = reader.GetReadEntities();

            if (readEntities.Count != 0)
               await BulkInsertSeparatedOwnedEntitiesAsync((IReadOnlyList<object>)readEntities, ctx, cancellationToken);
         }
      }
      finally
      {
         await _ctx.Database.CloseConnectionAsync().ConfigureAwait(false);
      }
   }

   private async Task BulkInsertSeparatedOwnedEntitiesAsync(
      IReadOnlyList<object> parentEntities,
      INpgsqlBulkOperationContext parentBulkOperationContext,
      CancellationToken cancellationToken)
   {
      if (parentEntities.Count == 0)
         return;

      foreach (var childContext in parentBulkOperationContext.GetChildren(parentEntities))
      {
         var childTableName = childContext.EntityType.GetTableName() ?? throw new InvalidOperationException($"The entity '{childContext.EntityType.Name}' has no table name.");

         await BulkInsertAsync(childContext.Entities,
                               childContext.EntityType.GetSchema(),
                               childTableName,
                               childContext,
                               cancellationToken).ConfigureAwait(false);
      }
   }

   private string SetColumnMappings(SqlBulkCopy bulkCopy, IEntityDataReader reader)
   {
      var columnsSb = _stringBuilderPool.Get();

      try
      {
         StoreObjectIdentifier? storeObject = null;

         for (var i = 0; i < reader.Properties.Count; i++)
         {
            var property = reader.Properties[i];

            storeObject ??= property.GetStoreObject();
            var columnName = property.GetColumnName(storeObject.Value);

            bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(i, columnName));

            if (columnsSb.Length > 0)
               columnsSb.Append(", ");

            columnsSb.Append(columnName).Append(' ').Append(property.Property.GetColumnType(storeObject.Value));
         }

         return columnsSb.ToString();
      }
      finally
      {
         _stringBuilderPool.Return(columnsSb);
      }
   }

   private SqlBulkCopy CreateSqlBulkCopy(SqlConnection sqlCon, SqlTransaction? sqlTx, string? schema, string tableName, NpgsqlBulkInsertOptions sqlServerOptions)
   {
      var bulkCopy = new SqlBulkCopy(sqlCon, sqlServerOptions.SqlBulkCopyOptions, sqlTx)
                     {
                        DestinationTableName = _sqlGenerationHelper.DelimitIdentifier(tableName, schema),
                        EnableStreaming = sqlServerOptions.EnableStreaming
                     };

      if (sqlServerOptions.BulkCopyTimeout.HasValue)
         bulkCopy.BulkCopyTimeout = (int)sqlServerOptions.BulkCopyTimeout.Value.TotalSeconds;

      if (sqlServerOptions.BatchSize.HasValue)
         bulkCopy.BatchSize = sqlServerOptions.BatchSize.Value;

      return bulkCopy;
   }

   private void LogInserting(SqlBulkCopyOptions options, SqlBulkCopy bulkCopy, string columns)
   {
      _logger.Logger.LogDebug(EventIds.Inserting,
                              """
                              Executing DbCommand [SqlBulkCopyOptions={SqlBulkCopyOptions}, BulkCopyTimeout={BulkCopyTimeout}, BatchSize={BatchSize}, EnableStreaming={EnableStreaming}]
                              INSERT BULK {Table} ({Columns})
                              """,
                              options,
                              bulkCopy.BulkCopyTimeout,
                              bulkCopy.BatchSize,
                              bulkCopy.EnableStreaming,
                              bulkCopy.DestinationTableName,
                              columns);
   }

   private void LogInserted(SqlBulkCopyOptions options, TimeSpan duration, SqlBulkCopy bulkCopy, string columns)
   {
      _logger.Logger.LogInformation(EventIds.Inserted,
                                    """
                                    Executed DbCommand ({Duration}ms) [SqlBulkCopyOptions={SqlBulkCopyOptions}, BulkCopyTimeout={BulkCopyTimeout}, BatchSize={BatchSize}, EnableStreaming={EnableStreaming}]
                                    INSERT BULK {Table} ({Columns})
                                    """,
                                    (long)duration.TotalMilliseconds,
                                    options,
                                    bulkCopy.BulkCopyTimeout,
                                    bulkCopy.BatchSize,
                                    bulkCopy.EnableStreaming,
                                    bulkCopy.DestinationTableName,
                                    columns);
   }


   /// <inheritdoc />
   public async Task TruncateTableAsync<T>(CancellationToken cancellationToken = default)
      where T : class
   {
      await TruncateTableAsync(typeof(T), cancellationToken);
   }

   /// <inheritdoc />
   public async Task TruncateTableAsync(Type type, CancellationToken cancellationToken = default)
   {
      var entityType = _ctx.Model.GetEntityType(type);
      var tableName = entityType.GetTableName() ?? throw new InvalidOperationException($"The entity '{entityType.Name}' has no table name.");

      var tableIdentifier = _sqlGenerationHelper.DelimitIdentifier(tableName, entityType.GetSchema());
      var truncateStatement = $"TRUNCATE TABLE {tableIdentifier};";

      await _ctx.Database.ExecuteSqlRawAsync(truncateStatement, cancellationToken);
   }
}
