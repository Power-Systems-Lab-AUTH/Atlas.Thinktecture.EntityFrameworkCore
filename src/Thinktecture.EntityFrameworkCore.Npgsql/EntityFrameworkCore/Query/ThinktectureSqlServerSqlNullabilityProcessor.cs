using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;
using Thinktecture.EntityFrameworkCore.Query.SqlExpressions;

namespace Thinktecture.EntityFrameworkCore.Query;

/// <summary>
/// Extends <see cref="NpgsqlSqlNullabilityProcessor"/>.
/// </summary>
[SuppressMessage("Usage", "EF1001", MessageId = "Internal EF Core API usage.")]
public class ThinktectureNpgsqlSqlNullabilityProcessor : NpgsqlSqlNullabilityProcessor
{
   /// <inheritdoc />
   public ThinktectureNpgsqlSqlNullabilityProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
      : base(dependencies, useRelationalNulls)
   {
   }

   /// <inheritdoc />
   protected override TableExpressionBase Visit(TableExpressionBase tableExpressionBase)
   {
      if (tableExpressionBase is INotNullableSqlExpression)
         return tableExpressionBase;

      return base.Visit(tableExpressionBase);
   }

   /// <inheritdoc />
   protected override SqlExpression VisitCustomSqlExpression(SqlExpression sqlExpression, bool allowOptimizedExpansion, out bool nullable)
   {
      if (sqlExpression is INotNullableSqlExpression)
      {
         nullable = false;
         return sqlExpression;
      }

      return base.VisitCustomSqlExpression(sqlExpression, allowOptimizedExpansion, out nullable);
   }
}
