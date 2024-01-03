using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Thinktecture.EntityFrameworkCore.Query;

/// <inheritdoc />
[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
public class ThinktectureNpgsqlQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
{
   private readonly QuerySqlGeneratorDependencies _dependencies;
   private readonly IRelationalTypeMappingSource _typeMappingSource;
   private readonly INpgsqlSingletonOptions _sqlServerSingletonOptions;
   private readonly ITenantDatabaseProviderFactory _databaseProviderFactory;

   /// <summary>
   /// Initializes new instance of <see cref="ThinktectureNpgsqlQuerySqlGeneratorFactory"/>.
   /// </summary>
   /// <param name="dependencies">Dependencies.</param>
   /// <param name="typeMappingSource">Type mapping source.</param>
   /// <param name="sqlServerSingletonOptions">Options.</param>
   /// <param name="databaseProviderFactory">Factory.</param>
   public ThinktectureNpgsqlQuerySqlGeneratorFactory(
      QuerySqlGeneratorDependencies dependencies,
      IRelationalTypeMappingSource typeMappingSource,
      INpgsqlSingletonOptions sqlServerSingletonOptions,
      ITenantDatabaseProviderFactory databaseProviderFactory)
   {
      _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
      _typeMappingSource = typeMappingSource ?? throw new ArgumentNullException(nameof(typeMappingSource));
      _sqlServerSingletonOptions = sqlServerSingletonOptions;
      _databaseProviderFactory = databaseProviderFactory ?? throw new ArgumentNullException(nameof(databaseProviderFactory));
   }

   /// <inheritdoc />
   public QuerySqlGenerator Create()
   {
      return new ThinktectureNpgsqlQuerySqlGenerator(_dependencies, _typeMappingSource, _sqlServerSingletonOptions, _databaseProviderFactory.Create());
   }
}
