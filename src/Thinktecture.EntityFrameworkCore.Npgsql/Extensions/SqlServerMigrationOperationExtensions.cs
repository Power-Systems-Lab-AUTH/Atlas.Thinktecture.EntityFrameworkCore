using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Thinktecture.EntityFrameworkCore.Migrations;

// ReSharper disable once CheckNamespace
namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="MigrationOperation"/>.
/// </summary>
public static class NpgsqlMigrationOperationExtensions
{
   /// <summary>
   /// Gets an indication whether the "IfNotExists" check is required.
   /// </summary>
   /// <remarks>
   /// The <see cref="ThinktectureNpgsqlMigrationsSqlGenerator"/> must be used so the annotations have some effect!
   /// Use the extension method "<see cref="NpgsqlDbContextOptionsBuilderExtensions.UseThinktectureNpgsqlMigrationsSqlGenerator"/>" to change the Migration SQL generator.
   /// </remarks>
   /// <param name="operation">Operation to check for the flag.</param>
   /// <returns><c>true</c> if the check is required; otherwise <c>false</c>.</returns>
   /// <exception cref="ArgumentNullException"><paramref name="operation"/> is <c>null</c>.</exception>
   public static bool IfNotExistsCheckRequired(this MigrationOperation operation)
   {
      ArgumentNullException.ThrowIfNull(operation);

      return operation[NpgsqlOperationBuilderExtensions.IfNotExistsKey] is bool ifNotExists && ifNotExists;
   }

   /// <summary>
   /// Gets an indication whether the "IfExists" check is required.
   /// </summary>
   /// <remarks>
   /// The <see cref="ThinktectureNpgsqlMigrationsSqlGenerator"/> must be used so the annotations have some effect!
   /// Use the extension method "<see cref="NpgsqlDbContextOptionsBuilderExtensions.UseThinktectureNpgsqlMigrationsSqlGenerator"/>" to change the Migration SQL generator.
   /// </remarks>
   /// <param name="operation">Operation to check for the flag.</param>
   /// <returns><c>true</c> if the check is required; otherwise <c>false</c>.</returns>
   /// <exception cref="ArgumentNullException"><paramref name="operation"/> is <c>null</c>.</exception>
   public static bool IfExistsCheckRequired(this MigrationOperation operation)
   {
      ArgumentNullException.ThrowIfNull(operation);

      return operation[NpgsqlOperationBuilderExtensions.IfExistsKey] is bool ifNotExists && ifNotExists;
   }
}
