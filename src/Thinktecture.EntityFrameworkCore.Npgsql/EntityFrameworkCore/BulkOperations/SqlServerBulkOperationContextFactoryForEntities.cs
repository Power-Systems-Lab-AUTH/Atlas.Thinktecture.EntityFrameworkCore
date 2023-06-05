using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Thinktecture.EntityFrameworkCore.Data;

namespace Thinktecture.EntityFrameworkCore.BulkOperations;

internal class NpgsqlBulkOperationContextFactoryForEntities : INpgsqlBulkOperationContextFactory
{
   public static readonly INpgsqlBulkOperationContextFactory Instance = new NpgsqlBulkOperationContextFactoryForEntities();

   public INpgsqlBulkOperationContext CreateForBulkInsert(DbContext ctx, NpgsqlBulkInsertOptions options, IReadOnlyList<PropertyWithNavigations> properties)
   {
      return new BulkInsertContext(ctx,
                                   ctx.GetService<IEntityDataReaderFactory>(),
                                   (SqlConnection)ctx.Database.GetDbConnection(),
                                   (SqlTransaction?)ctx.Database.CurrentTransaction?.GetDbTransaction(),
                                   options,
                                   properties);
   }
}
