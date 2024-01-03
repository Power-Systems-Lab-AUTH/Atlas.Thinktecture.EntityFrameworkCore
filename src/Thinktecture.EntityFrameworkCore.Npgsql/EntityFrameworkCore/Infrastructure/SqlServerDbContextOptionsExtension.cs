using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;
using Thinktecture.EntityFrameworkCore.BulkOperations;
using Thinktecture.EntityFrameworkCore.Migrations;
using Thinktecture.EntityFrameworkCore.Parameters;
using Thinktecture.EntityFrameworkCore.Query;
using Thinktecture.EntityFrameworkCore.Query.ExpressionTranslators;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

/// <summary>
/// Extensions for DbContextOptions.
/// </summary>
public sealed class NpgsqlDbContextOptionsExtension : DbContextOptionsExtensionBase, IDbContextOptionsExtension
{
   private readonly RelationalDbContextOptionsExtension _relationalOptions;

   private NpgsqlDbContextOptionsExtensionInfo? _info;

   /// <inheritdoc />
   public DbContextOptionsExtensionInfo Info => _info ??= new NpgsqlDbContextOptionsExtensionInfo(this);

   /// <summary>
   /// Enables and disables support for "RowNumber".
   /// </summary>
   [Obsolete($"Use '{nameof(AddWindowFunctionsSupport)}'")]
   public bool AddRowNumberSupport
   {
      get => _relationalOptions.AddWindowFunctionsSupport;
      set => _relationalOptions.AddWindowFunctionsSupport = value;
   }

   /// <summary>
   /// Enables and disables support for window functions like "RowNumber".
   /// </summary>
   public bool AddWindowFunctionsSupport
   {
      get => _relationalOptions.AddWindowFunctionsSupport;
      set => _relationalOptions.AddWindowFunctionsSupport = value;
   }

   /// <summary>
   /// Enables and disables tenants support.
   /// </summary>
   public bool AddTenantDatabaseSupport
   {
      get => _relationalOptions.AddTenantDatabaseSupport;
      set => _relationalOptions.AddTenantDatabaseSupport = value;
   }

   private bool _addCustomQueryableMethodTranslatingExpressionVisitorFactory;

   /// <summary>
   /// A custom factory is registered if <c>true</c>.
   /// The factory is required to be able to translate custom methods like <see cref="RelationalQueryableExtensions.AsSubQuery{TEntity}"/>.
   /// </summary>
   public bool AddCustomQueryableMethodTranslatingExpressionVisitorFactory
   {
      get => _addCustomQueryableMethodTranslatingExpressionVisitorFactory || AddBulkOperationSupport || AddWindowFunctionsSupport || AddTableHintSupport;
      set => _addCustomQueryableMethodTranslatingExpressionVisitorFactory = value;
   }

   private bool _addCustomRelationalParameterBasedSqlProcessorFactory;

   /// <summary>
   /// A custom factory is registered if <c>true</c>.
   /// The factory is required for some features.
   /// </summary>
   public bool AddCustomRelationalParameterBasedSqlProcessorFactory
   {
      get => _addCustomRelationalParameterBasedSqlProcessorFactory || AddBulkOperationSupport || AddWindowFunctionsSupport || AddTableHintSupport;
      set => _addCustomRelationalParameterBasedSqlProcessorFactory = value;
   }

   private bool _addCustomQuerySqlGeneratorFactory;

   /// <summary>
   /// A custom factory is registered if <c>true</c>.
   /// The factory is required for some features like 'tenant database support' or for generation of 'DELETE' statements.
   /// </summary>
   public bool AddCustomQuerySqlGeneratorFactory
   {
      get => _addCustomQuerySqlGeneratorFactory || AddBulkOperationSupport || AddTenantDatabaseSupport || AddWindowFunctionsSupport || AddTableHintSupport;
      set => _addCustomQuerySqlGeneratorFactory = value;
   }

   /// <summary>
   /// Enables and disables support for table hints.
   /// </summary>
   public bool AddTableHintSupport { get; set; }

   /// <summary>
   /// Enables and disables support for bulk operations and temp tables.
   /// </summary>
   public bool AddBulkOperationSupport { get; set; }

   /// <summary>
   /// Indication whether to configure temp tables for primitive types.
   /// </summary>
   public bool ConfigureTempTablesForPrimitiveTypes { get; set; }

   private JsonSerializerOptions? _collectionParameterJsonSerializerOptions;
   private bool _addCollectionParameterSupport;
   internal bool ConfigureCollectionParametersForPrimitiveTypes { get; private set; }
   internal bool UseDeferredCollectionParameterSerialization { get; private set; }

   /// <summary>
   /// Changes the implementation of <see cref="IMigrationsSqlGenerator"/> to <see cref="ThinktectureNpgsqlMigrationsSqlGenerator"/>.
   /// </summary>
   public bool UseThinktectureNpgsqlMigrationsSqlGenerator { get; set; }

   /// <summary>
   /// Initializes new instance of <see cref="NpgsqlDbContextOptionsExtension"/>.
   /// </summary>
   /// <param name="relationalOptions">An instance of <see cref="RelationalDbContextOptionsExtension"/>.</param>
   public NpgsqlDbContextOptionsExtension(RelationalDbContextOptionsExtension relationalOptions)
   {
      _relationalOptions = relationalOptions ?? throw new ArgumentNullException(nameof(relationalOptions));
   }

   /// <inheritdoc />
   [SuppressMessage("Usage", "EF1001", MessageId = "Internal EF Core API usage.")]
   public void ApplyServices(IServiceCollection services)
   {
      services.TryAddSingleton<NpgsqlDbContextOptionsExtensionOptions>();
      services.AddSingleton<IBulkOperationsDbContextOptionsExtensionOptions>(provider => provider.GetRequiredService<NpgsqlDbContextOptionsExtensionOptions>());
      services.AddSingleton<ISingletonOptions>(provider => provider.GetRequiredService<NpgsqlDbContextOptionsExtensionOptions>());

      services.Add<IMethodCallTranslatorPlugin, NpgsqlMethodCallTranslatorPlugin>(GetLifetime<IMethodCallTranslatorPlugin>());

      if (AddCustomQueryableMethodTranslatingExpressionVisitorFactory)
         AddWithCheck<IQueryableMethodTranslatingExpressionVisitorFactory, ThinktectureNpgsqlQueryableMethodTranslatingExpressionVisitorFactory, NpgsqlQueryableMethodTranslatingExpressionVisitorFactory>(services);
      if (AddCustomQuerySqlGeneratorFactory)
         AddWithCheck<IQuerySqlGeneratorFactory, ThinktectureNpgsqlQuerySqlGeneratorFactory, NpgsqlQuerySqlGeneratorFactory>(services);

      if (AddCustomRelationalParameterBasedSqlProcessorFactory)
         AddWithCheck<IRelationalParameterBasedSqlProcessorFactory, ThinktectureNpgsqlParameterBasedSqlProcessorFactory, NpgsqlParameterBasedSqlProcessorFactory>(services);

      if (AddBulkOperationSupport)
      {
         services.Add<IConventionSetPlugin, BulkOperationConventionSetPlugin>(GetLifetime<IConventionSetPlugin>());
         
         AddEntityDataReader(services);
         services.TryAddScoped<NpgsqlBulkOperationExecutor>();
         services.TryAddScoped<IBulkInsertExecutor>(provider => provider.GetRequiredService<NpgsqlBulkOperationExecutor>());
         services.TryAddScoped<ITruncateTableExecutor>(provider => provider.GetRequiredService<NpgsqlBulkOperationExecutor>());
      }

      if (_addCollectionParameterSupport)
      {
         var jsonSerializerOptions = _collectionParameterJsonSerializerOptions ?? new JsonSerializerOptions();

         services.AddSingleton<ICollectionParameterFactory>(serviceProvider => ActivatorUtilities.CreateInstance<NpgsqlCollectionParameterFactory>(serviceProvider, jsonSerializerOptions));
         services.Add<IConventionSetPlugin, NpgsqlCollectionParameterConventionSetPlugin>(GetLifetime<IConventionSetPlugin>());
      }

      if (UseThinktectureNpgsqlMigrationsSqlGenerator)
         AddWithCheck<IMigrationsSqlGenerator, ThinktectureNpgsqlMigrationsSqlGenerator, NpgsqlMigrationsSqlGenerator>(services);

      if (_relationalOptions.AddSchemaRespectingComponents)
         services.AddSingleton<IMigrationOperationSchemaSetter, NpgsqlMigrationOperationSchemaSetter>();
   }

   /// <summary>
   /// Registers a custom service with internal dependency injection container of Entity Framework Core.
   /// </summary>
   /// <param name="serviceType">Service type.</param>
   /// <param name="implementationType">Implementation type.</param>
   /// <param name="lifetime">Service lifetime.</param>
   /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="implementationType"/> is <c>null</c>.</exception>
   public void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime)
   {
      _relationalOptions.Register(serviceType, implementationType, lifetime);
   }

   /// <summary>
   /// Registers a custom service instance with internal dependency injection container of Entity Framework Core.
   /// </summary>
   /// <param name="serviceType">Service type.</param>
   /// <param name="implementationInstance">Implementation instance.</param>
   /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="implementationInstance"/> is <c>null</c>.</exception>
   public void Register(Type serviceType, object implementationInstance)
   {
      _relationalOptions.Register(serviceType, implementationInstance);
   }

   /// <summary>
   /// Enables and disables support for queryable parameters.
   /// </summary>
   public NpgsqlDbContextOptionsExtension AddCollectionParameterSupport(
      bool addCollectionParameterSupport,
      JsonSerializerOptions? jsonSerializerOptions,
      bool configureCollectionParametersForPrimitiveTypes,
      bool useDeferredSerialization)
   {
      _addCollectionParameterSupport = addCollectionParameterSupport;
      _collectionParameterJsonSerializerOptions = jsonSerializerOptions;
      ConfigureCollectionParametersForPrimitiveTypes = addCollectionParameterSupport && configureCollectionParametersForPrimitiveTypes;
      UseDeferredCollectionParameterSerialization = addCollectionParameterSupport && useDeferredSerialization;

      return this;
   }

   /// <inheritdoc />
   public void Validate(IDbContextOptions options)
   {
   }

   private class NpgsqlDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
   {
      private readonly NpgsqlDbContextOptionsExtension _extension;
      public override bool IsDatabaseProvider => false;

      private string? _logFragment;

      public override string LogFragment => _logFragment ??= CreateLogFragment();

      private string CreateLogFragment()
      {
         var sb = new StringBuilder();

         if (_extension.AddBulkOperationSupport)
            sb.Append("BulkOperationSupport ");

         if (_extension._addCollectionParameterSupport)
            sb.Append("CollectionParameterSupport ");

         if (_extension.AddTableHintSupport)
            sb.Append("TableHintSupport ");

         if (_extension.UseThinktectureNpgsqlMigrationsSqlGenerator)
            sb.Append("ThinktectureNpgsqlMigrationsSqlGenerator ");

         return sb.ToString();
      }

      /// <inheritdoc />
      public NpgsqlDbContextOptionsExtensionInfo(NpgsqlDbContextOptionsExtension extension)
         : base(extension)
      {
         _extension = extension ?? throw new ArgumentNullException(nameof(extension));
      }

      /// <inheritdoc />
      public override int GetServiceProviderHashCode()
      {
         var hashCode = new HashCode();

         hashCode.Add(_extension.AddCustomQueryableMethodTranslatingExpressionVisitorFactory);
         hashCode.Add(_extension.AddCustomQuerySqlGeneratorFactory);
         hashCode.Add(_extension.AddCustomRelationalParameterBasedSqlProcessorFactory);
         hashCode.Add(_extension.AddBulkOperationSupport);
         hashCode.Add(_extension.ConfigureTempTablesForPrimitiveTypes);
         hashCode.Add(_extension._addCollectionParameterSupport);
         hashCode.Add(_extension.ConfigureCollectionParametersForPrimitiveTypes);
         hashCode.Add(_extension.UseDeferredCollectionParameterSerialization);
         hashCode.Add(_extension._collectionParameterJsonSerializerOptions);
         hashCode.Add(_extension.AddTenantDatabaseSupport);
         hashCode.Add(_extension.AddTableHintSupport);
         hashCode.Add(_extension.UseThinktectureNpgsqlMigrationsSqlGenerator);

         return hashCode.ToHashCode();
      }

      /// <inheritdoc />
      public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
      {
         return other is NpgsqlDbContextOptionsExtensionInfo otherNpgsqlInfo
                && _extension.AddCustomQueryableMethodTranslatingExpressionVisitorFactory == otherNpgsqlInfo._extension.AddCustomQueryableMethodTranslatingExpressionVisitorFactory
                && _extension.AddCustomQuerySqlGeneratorFactory == otherNpgsqlInfo._extension.AddCustomQuerySqlGeneratorFactory
                && _extension.AddCustomRelationalParameterBasedSqlProcessorFactory == otherNpgsqlInfo._extension.AddCustomRelationalParameterBasedSqlProcessorFactory
                && _extension.AddBulkOperationSupport == otherNpgsqlInfo._extension.AddBulkOperationSupport
                && _extension.ConfigureTempTablesForPrimitiveTypes == otherNpgsqlInfo._extension.ConfigureTempTablesForPrimitiveTypes
                && _extension._addCollectionParameterSupport == otherNpgsqlInfo._extension._addCollectionParameterSupport
                && _extension.ConfigureCollectionParametersForPrimitiveTypes == otherNpgsqlInfo._extension.ConfigureCollectionParametersForPrimitiveTypes
                && _extension.UseDeferredCollectionParameterSerialization == otherNpgsqlInfo._extension.UseDeferredCollectionParameterSerialization
                && _extension._collectionParameterJsonSerializerOptions == otherNpgsqlInfo._extension._collectionParameterJsonSerializerOptions
                && _extension.AddTenantDatabaseSupport == otherNpgsqlInfo._extension.AddTenantDatabaseSupport
                && _extension.AddTableHintSupport == otherNpgsqlInfo._extension.AddTableHintSupport
                && _extension.UseThinktectureNpgsqlMigrationsSqlGenerator == otherNpgsqlInfo._extension.UseThinktectureNpgsqlMigrationsSqlGenerator;
      }

      /// <inheritdoc />
      public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
      {
         debugInfo["Thinktecture:CustomQueryableMethodTranslatingExpressionVisitorFactory"] = _extension.AddCustomQueryableMethodTranslatingExpressionVisitorFactory.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:CustomQuerySqlGeneratorFactory"] = _extension.AddCustomQuerySqlGeneratorFactory.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:CustomRelationalParameterBasedSqlProcessorFactory"] = _extension.AddCustomRelationalParameterBasedSqlProcessorFactory.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:BulkOperationSupport"] = _extension.AddBulkOperationSupport.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:CollectionParameterSupport"] = _extension._addCollectionParameterSupport.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:TenantDatabaseSupport"] = _extension.AddTenantDatabaseSupport.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:TableHintSupport"] = _extension.AddTableHintSupport.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:UseThinktectureNpgsqlMigrationsSqlGenerator"] = _extension.UseThinktectureNpgsqlMigrationsSqlGenerator.ToString(CultureInfo.InvariantCulture);
      }
   }
}
