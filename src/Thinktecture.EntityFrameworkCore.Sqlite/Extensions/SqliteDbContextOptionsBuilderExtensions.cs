using Microsoft.EntityFrameworkCore.Infrastructure;
using Thinktecture.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="SqliteDbContextOptionsBuilder"/>.
/// </summary>
public static class SqliteDbContextOptionsBuilderExtensions
{
   /// <summary>
   /// Adds support for bulk operations and temp tables.
   /// </summary>
   /// <param name="sqliteOptionsBuilder">SQLite options builder.</param>
   /// <param name="addBulkOperationSupport">Indication whether to enable or disable the feature.</param>
   /// <param name="configureTempTablesForPrimitiveTypes">Indication whether to configure temp tables for primitive types.</param>
   /// <returns>Provided <paramref name="sqliteOptionsBuilder"/>.</returns>
   public static SqliteDbContextOptionsBuilder AddBulkOperationSupport(
      this SqliteDbContextOptionsBuilder sqliteOptionsBuilder,
      bool addBulkOperationSupport = true,
      bool configureTempTablesForPrimitiveTypes = true)
   {
      return AddOrUpdateExtension(sqliteOptionsBuilder, extension =>
                                                        {
                                                           extension.AddBulkOperationSupport = addBulkOperationSupport;
                                                           extension.ConfigureTempTablesForPrimitiveTypes = addBulkOperationSupport && configureTempTablesForPrimitiveTypes;
                                                           return extension;
                                                        });
   }

   /// <summary>
   /// Adds custom factory required for translation of custom methods like <see cref="RelationalQueryableExtensions.AsSubQuery{TEntity}"/>.
   /// </summary>
   /// <param name="builder">Options builder.</param>
   /// <param name="addCustomQueryableMethodTranslatingExpressionVisitorFactory">Indication whether to add a custom factory.</param>
   /// <returns>Provided <paramref name="builder"/>.</returns>
   public static SqliteDbContextOptionsBuilder AddCustomQueryableMethodTranslatingExpressionVisitorFactory(
      this SqliteDbContextOptionsBuilder builder,
      bool addCustomQueryableMethodTranslatingExpressionVisitorFactory = true)
   {
      builder.AddOrUpdateExtension(extension =>
                                   {
                                      extension.AddCustomQueryableMethodTranslatingExpressionVisitorFactory = addCustomQueryableMethodTranslatingExpressionVisitorFactory;
                                      return extension;
                                   });
      return builder;
   }

   /// <summary>
   /// Adds support for "RowNumber".
   /// </summary>
   /// <param name="builder">Options builder.</param>
   /// <param name="addRowNumberSupport">Indication whether to enable or disable the feature.</param>
   /// <returns>Provided <paramref name="builder"/>.</returns>
   [Obsolete($"Use '{nameof(AddWindowFunctionsSupport)}' instead.")]
   public static SqliteDbContextOptionsBuilder AddRowNumberSupport(
      this SqliteDbContextOptionsBuilder builder,
      bool addRowNumberSupport = true)
   {
      builder.AddOrUpdateExtension(extension =>
                                   {
                                      extension.AddWindowFunctionsSupport = addRowNumberSupport;
                                      return extension;
                                   });
      return builder;
   }

   /// <summary>
   /// Adds support for window functions like "RowNumber".
   /// </summary>
   /// <param name="builder">Options builder.</param>
   /// <param name="addWindowFunctionsSupport">Indication whether to enable or disable the feature.</param>
   /// <returns>Provided <paramref name="builder"/>.</returns>
   public static SqliteDbContextOptionsBuilder AddWindowFunctionsSupport(
      this SqliteDbContextOptionsBuilder builder,
      bool addWindowFunctionsSupport = true)
   {
      builder.AddOrUpdateExtension(extension =>
                                   {
                                      extension.AddWindowFunctionsSupport = addWindowFunctionsSupport;
                                      return extension;
                                   });
      return builder;
   }

   private static SqliteDbContextOptionsBuilder AddOrUpdateExtension(this SqliteDbContextOptionsBuilder sqliteOptionsBuilder,
                                                                     Func<SqliteDbContextOptionsExtension, SqliteDbContextOptionsExtension> callback)
   {
      ArgumentNullException.ThrowIfNull(sqliteOptionsBuilder);

      var infrastructure = (IRelationalDbContextOptionsBuilderInfrastructure)sqliteOptionsBuilder;
      var relationalOptions = infrastructure.OptionsBuilder.TryAddExtension<RelationalDbContextOptionsExtension>();
      infrastructure.OptionsBuilder.AddOrUpdateExtension(callback, () => new SqliteDbContextOptionsExtension(relationalOptions));

      return sqliteOptionsBuilder;
   }
}
