using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Thinktecture.EntityFrameworkCore.Data;

namespace Thinktecture.EntityFrameworkCore.BulkOperations;

internal class BulkInsertContext : INpgsqlBulkOperationContext
{
   private readonly IReadOnlyList<PropertyWithNavigations> _externalProperties;
   private readonly DbContext _ctx;
   private readonly IEntityDataReaderFactory _readerFactory;

   public SqlConnection Connection { get; }
   public SqlTransaction? Transaction { get; }
   public IReadOnlyList<PropertyWithNavigations> Properties { get; }
   public NpgsqlBulkInsertOptions Options { get; }
   public bool HasExternalProperties => _externalProperties.Count != 0;

   public IEntityDataReader<T> CreateReader<T>(IEnumerable<T> entities)
   {
      return _readerFactory.Create(_ctx, entities, Properties, HasExternalProperties);
   }

   public BulkInsertContext(
      DbContext ctx,
      IEntityDataReaderFactory factory,
      SqlConnection connection,
      SqlTransaction? transaction,
      NpgsqlBulkInsertOptions options,
      IReadOnlyList<PropertyWithNavigations> properties)
   {
      _ctx = ctx;
      _readerFactory = factory;
      Connection = connection;
      Transaction = transaction;
      var (ownProperties, externalProperties) = properties.SeparateProperties();
      Properties = ownProperties;
      _externalProperties = externalProperties;
      Options = options;
   }

   public IReadOnlyList<INpgsqlOwnedTypeBulkOperationContext> GetChildren(IReadOnlyList<object> entities)
   {
      if (!HasExternalProperties)
         return Array.Empty<INpgsqlOwnedTypeBulkOperationContext>();

      var childCtx = new List<INpgsqlOwnedTypeBulkOperationContext>();

      foreach (var (navigation, ownedEntities, properties) in _externalProperties.GroupExternalProperties(entities))
      {
         var ownedTypeCtx = new OwnedTypeBulkInsertContext(_ctx, _readerFactory, Connection, Transaction, Options, properties, navigation.TargetEntityType, ownedEntities);
         childCtx.Add(ownedTypeCtx);
      }

      return childCtx;
   }

   private class OwnedTypeBulkInsertContext : BulkInsertContext, INpgsqlOwnedTypeBulkOperationContext
   {
      public IEntityType EntityType { get; }
      public IEnumerable<object> Entities { get; }

      public OwnedTypeBulkInsertContext(
         DbContext ctx,
         IEntityDataReaderFactory factory,
         SqlConnection connection,
         SqlTransaction? transaction,
         NpgsqlBulkInsertOptions options,
         IReadOnlyList<PropertyWithNavigations> properties,
         IEntityType entityType,
         IEnumerable<object> entities)
         : base(ctx, factory, connection, transaction, options, properties)
      {
         EntityType = entityType;
         Entities = entities;
      }
   }
}
