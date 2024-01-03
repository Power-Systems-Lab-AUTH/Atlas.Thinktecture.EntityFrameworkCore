using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;
using Thinktecture.EntityFrameworkCore.Query.SqlExpressions;

namespace Thinktecture.EntityFrameworkCore.Query.ExpressionTranslators;

/// <summary>
/// Translates extension methods like "RowNumber"
/// </summary>
public sealed class NpgsqlDbFunctionsTranslator : IMethodCallTranslator
{
   private readonly ISqlExpressionFactory _sqlExpressionFactory;

   internal NpgsqlDbFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory)
   {
      _sqlExpressionFactory = sqlExpressionFactory;
   }

   /// <inheritdoc />
   public SqlExpression? Translate(
      SqlExpression? instance,
      MethodInfo method,
      IReadOnlyList<SqlExpression> arguments,
      IDiagnosticsLogger<DbLoggerCategory.Query> logger)
   {
      ArgumentNullException.ThrowIfNull(method);
      ArgumentNullException.ThrowIfNull(arguments);

      if (method.DeclaringType != typeof(NpgsqlDbFunctionsExtensions))
         return null;

      switch (method.Name)
      {
         case "Sum":
         {
            return CreateWindowFunctionExpression("SUM", arguments);
         }
         case "Average":
         {
            return CreateWindowFunctionExpression("AVG", arguments);
         }
         case "Max":
         {
            return CreateWindowFunctionExpression("MAX", arguments);
         }
         case "Min":
         {
            return CreateWindowFunctionExpression("MIN", arguments);
         }
         default:
            throw new InvalidOperationException($"Unexpected method '{method.Name}' in '{nameof(NpgsqlDbFunctionsExtensions)}'.");
      }
   }

   [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
   private SqlExpression CreateWindowFunctionExpression(string aggregateFunction, IReadOnlyList<SqlExpression> arguments)
   {
      var expression = arguments[0];
      var partitionBy = arguments.Count > 1 ? arguments[1] : null;
      var orderBy = arguments.Count > 2 ? arguments.Skip(2).Select(a => new OrderingExpression(a, true)).ToList() : null;

      var aggregateFunctionExpression = _sqlExpressionFactory.Function(
         aggregateFunction,
         new[] { expression },
         nullable: true,
         argumentsPropagateNullability: new[] { true },
         typeof(decimal));

      return new WindowFunctionExpression(aggregateFunctionExpression, partitionBy == null ? null : new [] { partitionBy }, orderBy);
   }
}
