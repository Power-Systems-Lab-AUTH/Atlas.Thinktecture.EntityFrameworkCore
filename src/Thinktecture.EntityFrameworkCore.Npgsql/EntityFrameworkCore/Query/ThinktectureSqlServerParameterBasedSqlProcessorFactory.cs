using Microsoft.EntityFrameworkCore.Query;

namespace Thinktecture.EntityFrameworkCore.Query;

/// <inheritdoc />
public class ThinktectureNpgsqlParameterBasedSqlProcessorFactory : IRelationalParameterBasedSqlProcessorFactory
{
   private readonly RelationalParameterBasedSqlProcessorDependencies _dependencies;

   /// <summary>
   /// Initializes <see cref="ThinktectureNpgsqlParameterBasedSqlProcessorFactory"/>.
   /// </summary>
   /// <param name="dependencies">Dependencies.</param>
   public ThinktectureNpgsqlParameterBasedSqlProcessorFactory(
      RelationalParameterBasedSqlProcessorDependencies dependencies)
   {
      _dependencies = dependencies;
   }

   /// <inheritdoc />
   public RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
   {
      return new ThinktectureNpgsqlParameterBasedSqlProcessor(_dependencies, useRelationalNulls);
   }
}
