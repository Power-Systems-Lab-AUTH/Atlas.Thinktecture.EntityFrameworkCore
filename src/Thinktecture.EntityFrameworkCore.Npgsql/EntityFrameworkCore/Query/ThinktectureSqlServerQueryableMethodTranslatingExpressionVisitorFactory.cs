using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;

namespace Thinktecture.EntityFrameworkCore.Query;

/// <summary>
/// Factory for creation of the <see cref="ThinktectureNpgsqlQueryableMethodTranslatingExpressionVisitor"/>.
/// </summary>
[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
public sealed class ThinktectureNpgsqlQueryableMethodTranslatingExpressionVisitorFactory
   : IQueryableMethodTranslatingExpressionVisitorFactory
{
   private readonly QueryableMethodTranslatingExpressionVisitorDependencies _dependencies;
   private readonly RelationalQueryableMethodTranslatingExpressionVisitorDependencies _relationalDependencies;

   /// <summary>
   /// Initializes new instance of <see cref="ThinktectureNpgsqlQueryableMethodTranslatingExpressionVisitorFactory"/>.
   /// </summary>
   /// <param name="dependencies">Dependencies.</param>
   /// <param name="relationalDependencies">Relational dependencies.</param>
   public ThinktectureNpgsqlQueryableMethodTranslatingExpressionVisitorFactory(
      QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
      RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies)
   {
      _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
      _relationalDependencies = relationalDependencies ?? throw new ArgumentNullException(nameof(relationalDependencies));
   }

   /// <inheritdoc />
   public QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
   {
      return new ThinktectureNpgsqlQueryableMethodTranslatingExpressionVisitor(_dependencies, _relationalDependencies, queryCompilationContext);
   }
}
