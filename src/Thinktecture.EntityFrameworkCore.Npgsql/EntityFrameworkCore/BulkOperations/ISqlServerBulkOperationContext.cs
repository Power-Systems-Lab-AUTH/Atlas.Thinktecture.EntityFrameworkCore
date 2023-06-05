using Microsoft.Data.SqlClient;

namespace Thinktecture.EntityFrameworkCore.BulkOperations;

internal interface INpgsqlBulkOperationContext : IBulkOperationContext
{
   SqlConnection Connection { get; }
   SqlTransaction? Transaction { get; }
   NpgsqlBulkInsertOptions Options { get; }

   IReadOnlyList<INpgsqlOwnedTypeBulkOperationContext> GetChildren(IReadOnlyList<object> entities);
}
