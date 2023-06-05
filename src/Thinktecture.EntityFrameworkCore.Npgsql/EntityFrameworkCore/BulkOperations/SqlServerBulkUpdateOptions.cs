using Microsoft.Data.SqlClient;

namespace Thinktecture.EntityFrameworkCore.BulkOperations;

/// <summary>
/// Bulk update options for SQL Server.
/// </summary>
public sealed class NpgsqlBulkUpdateOptions : INpgsqlMergeOperationOptions, IBulkUpdateOptions
{
   /// <inheritdoc />
   public IEntityPropertiesProvider? PropertiesToUpdate { get; set; }

   /// <inheritdoc />
   public IEntityPropertiesProvider? KeyProperties { get; set; }

   /// <inheritdoc />
   public List<NpgsqlTableHintLimited> MergeTableHints { get; }

   /// <summary>
   /// Initializes new instance of <see cref="NpgsqlBulkUpdateOptions"/>.
   /// </summary>
   /// <param name="optionsToInitializeFrom">Options to initialize from.</param>
   public NpgsqlBulkUpdateOptions(IBulkUpdateOptions? optionsToInitializeFrom = null)
   {
      if (optionsToInitializeFrom is not null)
      {
         PropertiesToUpdate = optionsToInitializeFrom.PropertiesToUpdate;
         KeyProperties = optionsToInitializeFrom.KeyProperties;
      }

      if (optionsToInitializeFrom is INpgsqlMergeOperationOptions mergeOptions)
      {
         MergeTableHints = mergeOptions.MergeTableHints.ToList();
      }
      else
      {
         MergeTableHints = new List<NpgsqlTableHintLimited> { NpgsqlTableHintLimited.HoldLock };
      }
   }
}
