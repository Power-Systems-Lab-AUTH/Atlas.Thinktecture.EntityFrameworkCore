using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Thinktecture.EntityFrameworkCore.Data;

namespace Thinktecture.EntityFrameworkCore.BulkOperations;

internal class NpgsqlBulkOperationContextFactoryForValues : INpgsqlBulkOperationContextFactory
{
   public static readonly INpgsqlBulkOperationContextFactory Instance = new NpgsqlBulkOperationContextFactoryForValues();

   public INpgsqlBulkOperationContext CreateForBulkInsert(DbContext ctx, NpgsqlBulkInsertOptions options, IReadOnlyList<PropertyWithNavigations> properties)
   {
      return new BulkInsertValueContext(properties,
                                        (SqlConnection)ctx.Database.GetDbConnection(),
                                        (SqlTransaction?)ctx.Database.CurrentTransaction?.GetDbTransaction(),
                                        options);
   }
}
