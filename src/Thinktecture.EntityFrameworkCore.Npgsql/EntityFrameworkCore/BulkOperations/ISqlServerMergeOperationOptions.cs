namespace Thinktecture.EntityFrameworkCore.BulkOperations;

/// <summary>
/// Options for SQL Server 'MERGE' operation.
/// </summary>
public interface INpgsqlMergeOperationOptions
{
   /// <summary>
   /// Table hints for the MERGE command.
   /// </summary>
   List<NpgsqlTableHintLimited> MergeTableHints { get; }
}
