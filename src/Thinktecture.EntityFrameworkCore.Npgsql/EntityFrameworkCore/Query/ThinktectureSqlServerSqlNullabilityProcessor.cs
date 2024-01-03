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
   protected override SqlExpression VisitCustomSqlExpression(SqlExpression sqlExpression, bool allowOptimizedExpansion, out bool nullable)
   {
      switch (sqlExpression)
      {
         case INotNullableSqlExpression:
         {
            nullable = false;
            return sqlExpression;
         }
         //case WindowFunctionExpression { AggregateFunction: NpgsqlAggregateFunctionExpression aggregateFunction } windowFunctionExpression:
         //{
         //   var visitedAggregateFunction = base.VisitNpgsqlAggregateFunction(aggregateFunction, allowOptimizedExpansion, out nullable);
//
         //   return aggregateFunction == visitedAggregateFunction
         //             ? windowFunctionExpression
         //             : new WindowFunctionExpression(visitedAggregateFunction, windowFunctionExpression.Partitions, windowFunctionExpression.Orderings);
         //}
         default:
            return base.VisitCustomSqlExpression(sqlExpression, allowOptimizedExpansion, out nullable);
      }
   }
}
