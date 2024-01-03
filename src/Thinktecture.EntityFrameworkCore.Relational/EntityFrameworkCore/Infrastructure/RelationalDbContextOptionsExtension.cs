using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Thinktecture.EntityFrameworkCore.Migrations;
using Thinktecture.EntityFrameworkCore.Query;
using Thinktecture.EntityFrameworkCore.Query.ExpressionTranslators;
using Thinktecture.EntityFrameworkCore.Storage;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

/// <summary>
/// Extensions for DbContextOptions.
/// </summary>
public sealed class RelationalDbContextOptionsExtension : DbContextOptionsExtensionBase, IDbContextOptionsExtension
{
   private const int _DEFAULT_INITIAL_CAPACITY = 300;
   private const int _DEFAULT_MAXIMUM_RETAINED_CAPACITY = 4 * 1024;

   private static readonly IRelationalDbContextComponentDecorator _defaultDecorator = new RelationalDbContextComponentDecorator();

   private readonly List<ServiceDescriptor> _serviceDescriptors;
   private readonly List<Type> _evaluatableExpressionFilterPlugins;
   private readonly StringBuilderPooledObjectPolicy _stringBuilderPolicy;

   private DbContextOptionsExtensionInfo? _info;

   /// <inheritdoc />
   public DbContextOptionsExtensionInfo Info => _info ??= new RelationalDbContextOptionsExtensionInfo(this);

   /// <summary>
   /// Adds components so Entity Framework Core can handle changes of the database schema at runtime.
   /// </summary>
   public bool AddSchemaRespectingComponents { get; set; }

   private IRelationalDbContextComponentDecorator? _componentDecorator;

   /// <summary>
   /// Decorates components.
   /// </summary>
   public IRelationalDbContextComponentDecorator ComponentDecorator
   {
      get => _componentDecorator ?? _defaultDecorator;
      set => _componentDecorator = value;
   }

   /// <summary>
   /// Adds support for nested transactions.
   /// </summary>
   public bool AddNestedTransactionsSupport { get; set; }

   /// <summary>
   /// Enables and disables support for windows functions like "RowNumber".
   /// </summary>
   public bool AddWindowFunctionsSupport { get; set; }

   /// <summary>
   /// Enables and disables support for 'tenant database support'.
   /// </summary>
   public bool AddTenantDatabaseSupport { get; set; }

   private bool _useCustomRelationalQueryContextFactory;

   /// <summary>
   /// A custom factory is registered if <c>true</c>.
   /// The factory is required for some features.
   /// </summary>
   public bool UseThinktectureRelationalQueryContextFactory
   {
      get => _useCustomRelationalQueryContextFactory || AddTenantDatabaseSupport;
      set => _useCustomRelationalQueryContextFactory = value;
   }

   /// <summary>
   /// Initializes new instance of <see cref="RelationalDbContextOptionsExtension"/>.
   /// </summary>
   public RelationalDbContextOptionsExtension()
   {
      _serviceDescriptors = new List<ServiceDescriptor>();
      _evaluatableExpressionFilterPlugins = new List<Type>();
      _stringBuilderPolicy = new StringBuilderPooledObjectPolicy
                             {
                                InitialCapacity = _DEFAULT_INITIAL_CAPACITY,
                                MaximumRetainedCapacity = _DEFAULT_MAXIMUM_RETAINED_CAPACITY
                             };
   }

   /// <inheritdoc />
   public void ApplyServices(IServiceCollection services)
   {
      ArgumentNullException.ThrowIfNull(services);

      services.TryAddSingleton<RelationalDbContextOptionsExtensionOptions>();
      services.AddSingleton<ISingletonOptions>(provider => provider.GetRequiredService<RelationalDbContextOptionsExtensionOptions>());

      services.TryAddSingleton<ITenantDatabaseProviderFactory>(DummyTenantDatabaseProviderFactory.Instance);

      services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
      services.TryAddSingleton(serviceProvider =>
                               {
                                  var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                                  return provider.Create(_stringBuilderPolicy);
                               });

      services.Add<IMethodCallTranslatorPlugin, RelationalMethodCallTranslatorPlugin>(GetLifetime<IMethodCallTranslatorPlugin>());

      if (UseThinktectureRelationalQueryContextFactory)
         ComponentDecorator.RegisterDecorator<IQueryContextFactory>(services, typeof(ThinktectureRelationalQueryContextFactory<>));

      if (_evaluatableExpressionFilterPlugins.Count > 0)
      {
         var lifetime = GetLifetime<IEvaluatableExpressionFilterPlugin>();

         foreach (var plugin in _evaluatableExpressionFilterPlugins)
         {
            services.Add(ServiceDescriptor.Describe(typeof(IEvaluatableExpressionFilterPlugin), plugin, lifetime));
         }
      }

      if (AddSchemaRespectingComponents)
         RegisterDefaultSchemaRespectingComponents(services);

      if (AddNestedTransactionsSupport)
         services.Add(ServiceDescriptor.Describe(typeof(IDbContextTransactionManager), typeof(NestedRelationalTransactionManager), GetLifetime<IDbContextTransactionManager>()));

      foreach (var descriptor in _serviceDescriptors)
      {
         services.Add(descriptor);
      }
   }

   private void RegisterDefaultSchemaRespectingComponents(IServiceCollection services)
   {
      services.TryAddSingleton<IMigrationOperationSchemaSetter, MigrationOperationSchemaSetter>();

      ComponentDecorator.RegisterDecorator<IModelCacheKeyFactory>(services, typeof(DefaultSchemaRespectingModelCacheKeyFactory<>));
      ComponentDecorator.RegisterDecorator<IModelCustomizer>(services, typeof(DefaultSchemaModelCustomizer<>));
      ComponentDecorator.RegisterDecorator<IMigrationsAssembly>(services, typeof(DefaultSchemaRespectingMigrationAssembly<>));
   }

   /// <inheritdoc />
   public void Validate(IDbContextOptions options)
   {
      if (AddTenantDatabaseSupport && _serviceDescriptors.All(d => d.ServiceType != typeof(ITenantDatabaseProviderFactory)))
         throw new InvalidOperationException($"TenantDatabaseSupport is enabled but there is no registration of an implementation of '{nameof(ITenantDatabaseProviderFactory)}'.");
   }

   /// <summary>
   /// Configures the string builder pool.
   /// </summary>
   /// <param name="initialCapacity">Initial capacity of a new <see cref="StringBuilder"/>.</param>
   /// <param name="maximumRetainedCapacity">Instances of <see cref="StringBuilder"/> with greater capacity are not reused.</param>
   /// <exception cref="ArgumentOutOfRangeException">If <paramref name="initialCapacity"/> or <paramref name="maximumRetainedCapacity"/> are negative.</exception>
   public RelationalDbContextOptionsExtension ConfigureStringBuilderPool(int initialCapacity, int maximumRetainedCapacity)
   {
      if (initialCapacity < 0)
         throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity cannot be negative.");

      if (maximumRetainedCapacity < 0)
         throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity cannot be negative.");

      _stringBuilderPolicy.InitialCapacity = initialCapacity;
      _stringBuilderPolicy.MaximumRetainedCapacity = maximumRetainedCapacity;

      return this;
   }

   /// <summary>
   /// Adds provided <paramref name="type"/> to dependency injection.
   /// </summary>
   /// <param name="type">An implementation of <see cref="IRelationalTypeMappingSourcePlugin"/>.</param>
   /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
   public RelationalDbContextOptionsExtension AddRelationalTypeMappingSourcePlugin(Type type)
   {
      ArgumentNullException.ThrowIfNull(type);

      if (!typeof(IRelationalTypeMappingSourcePlugin).IsAssignableFrom(type))
         throw new ArgumentException($"The provided type '{type.ShortDisplayName()}' must implement '{nameof(IRelationalTypeMappingSourcePlugin)}'.", nameof(type));

      Register(typeof(IRelationalTypeMappingSourcePlugin), type, ServiceLifetime.Singleton);

      return this;
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
      ArgumentNullException.ThrowIfNull(serviceType);

      ArgumentNullException.ThrowIfNull(implementationType);

      _serviceDescriptors.Add(ServiceDescriptor.Describe(serviceType, implementationType, lifetime));
   }

   /// <summary>
   /// Registers a custom service instance with internal dependency injection container of Entity Framework Core.
   /// </summary>
   /// <param name="serviceType">Service type.</param>
   /// <param name="implementationInstance">Implementation instance.</param>
   /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="implementationInstance"/> is <c>null</c>.</exception>
   public void Register(Type serviceType, object implementationInstance)
   {
      ArgumentNullException.ThrowIfNull(serviceType);

      ArgumentNullException.ThrowIfNull(implementationInstance);

      _serviceDescriptors.Add(ServiceDescriptor.Singleton(serviceType, implementationInstance));
   }

   /// <summary>
   /// Adds an <see cref="IEvaluatableExpressionFilterPlugin"/> to the dependency injection.
   /// </summary>
   /// <typeparam name="T">Type of the plugin.</typeparam>
   public RelationalDbContextOptionsExtension AddEvaluatableExpressionFilterPlugin<T>()
      where T : IEvaluatableExpressionFilterPlugin
   {
      var type = typeof(T);

      if (!_evaluatableExpressionFilterPlugins.Contains(type))
         _evaluatableExpressionFilterPlugins.Add(type);

      return this;
   }

   private class RelationalDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
   {
      private readonly RelationalDbContextOptionsExtension _extension;

      public override bool IsDatabaseProvider => false;

      private string? _logFragment;

      public override string LogFragment => _logFragment ??= CreateLogFragment();

      private string CreateLogFragment()
      {
         var sb = new StringBuilder();

         if (_extension.AddSchemaRespectingComponents)
            sb.Append("SchemaRespectingComponents ");

         if (_extension.AddNestedTransactionsSupport)
            sb.Append("NestedTransactionsSupport ");

         if (_extension.AddWindowFunctionsSupport)
            sb.Append("WindowFunctionsSupport ");

         if (_extension.AddTenantDatabaseSupport)
            sb.Append("TenantDatabaseSupport ");

         if (_extension._stringBuilderPolicy.InitialCapacity != _DEFAULT_INITIAL_CAPACITY || _extension._stringBuilderPolicy.MaximumRetainedCapacity != _DEFAULT_MAXIMUM_RETAINED_CAPACITY)
            sb.Append("StringBuilderPool(InitialCapacity=").Append(_extension._stringBuilderPolicy.InitialCapacity).Append(", MaximumRetainedCapacity=").Append(_extension._stringBuilderPolicy.MaximumRetainedCapacity).Append(") ");

         return sb.ToString();
      }

      public RelationalDbContextOptionsExtensionInfo(RelationalDbContextOptionsExtension extension)
         : base(extension)
      {
         _extension = extension ?? throw new ArgumentNullException(nameof(extension));
      }

      public override int GetServiceProviderHashCode()
      {
         var hashCode = new HashCode();
         hashCode.Add(_extension.UseThinktectureRelationalQueryContextFactory);
         hashCode.Add(_extension.AddSchemaRespectingComponents);
         hashCode.Add(_extension.AddNestedTransactionsSupport);
         hashCode.Add(_extension.AddTenantDatabaseSupport);
         hashCode.Add(_extension.AddWindowFunctionsSupport);
         hashCode.Add(_extension.ComponentDecorator);
         hashCode.Add(_extension._stringBuilderPolicy.InitialCapacity);
         hashCode.Add(_extension._stringBuilderPolicy.MaximumRetainedCapacity);

         _extension._evaluatableExpressionFilterPlugins.ForEach(type => hashCode.Add(type));
         _extension._serviceDescriptors.ForEach(descriptor => hashCode.Add(GetHashCode(descriptor)));

         // Following switches doesn't add any new components:
         //   AddCustomRelationalParameterBasedSqlProcessorFactory

         return hashCode.ToHashCode();
      }

      /// <inheritdoc />
      public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
      {
         if (other is not RelationalDbContextOptionsExtensionInfo otherRelationalInfo)
            return false;

         var areEqual = _extension.UseThinktectureRelationalQueryContextFactory == otherRelationalInfo._extension.UseThinktectureRelationalQueryContextFactory
                        && _extension.AddSchemaRespectingComponents == otherRelationalInfo._extension.AddSchemaRespectingComponents
                        && _extension.AddNestedTransactionsSupport == otherRelationalInfo._extension.AddNestedTransactionsSupport
                        && _extension.AddTenantDatabaseSupport == otherRelationalInfo._extension.AddTenantDatabaseSupport
                        && _extension.AddWindowFunctionsSupport == otherRelationalInfo._extension.AddWindowFunctionsSupport
                        && _extension._stringBuilderPolicy.InitialCapacity == otherRelationalInfo._extension._stringBuilderPolicy.InitialCapacity
                        && _extension._stringBuilderPolicy.MaximumRetainedCapacity == otherRelationalInfo._extension._stringBuilderPolicy.MaximumRetainedCapacity
                        && _extension.ComponentDecorator.Equals(otherRelationalInfo._extension.ComponentDecorator);

         if (!areEqual)
            return false;

         if (_extension._evaluatableExpressionFilterPlugins.Count != otherRelationalInfo._extension._evaluatableExpressionFilterPlugins.Count)
            return false;

         if (_extension._evaluatableExpressionFilterPlugins.Except(otherRelationalInfo._extension._evaluatableExpressionFilterPlugins).Any())
            return false;

         if (_extension._serviceDescriptors.Count != otherRelationalInfo._extension._serviceDescriptors.Count)
            return false;

         if (!_extension._serviceDescriptors.All(d => otherRelationalInfo._extension._serviceDescriptors.Any(o => AreEqual(d, o))))
            return false;

         return true;
      }

      private static bool AreEqual(ServiceDescriptor serviceDescriptor, ServiceDescriptor other)
      {
         if (serviceDescriptor.Lifetime != other.Lifetime || serviceDescriptor.ServiceType != other.ServiceType)
            return false;

         if (serviceDescriptor.ImplementationType is not null)
            return serviceDescriptor.ImplementationType == other.ImplementationType;

         if (serviceDescriptor.ImplementationInstance is not null)
            return serviceDescriptor.ImplementationInstance.Equals(other.ImplementationInstance);

         throw new NotSupportedException("Implementation factories are not supported.");
      }

      private static int GetHashCode(ServiceDescriptor descriptor)
      {
         int implHashcode;

         if (descriptor.ImplementationType != null)
         {
            implHashcode = descriptor.ImplementationType.GetHashCode();
         }
         else if (descriptor.ImplementationInstance != null)
         {
            implHashcode = descriptor.ImplementationInstance.GetHashCode();
         }
         else
         {
            throw new NotSupportedException("Implementation factories are not supported.");
         }

         return HashCode.Combine(descriptor.Lifetime, descriptor.ServiceType, implHashcode);
      }

      public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
      {
         debugInfo["Thinktecture:CustomRelationalQueryContextFactory"] = _extension.UseThinktectureRelationalQueryContextFactory.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:SchemaRespectingComponents"] = _extension.AddSchemaRespectingComponents.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:NestedTransactionsSupport"] = _extension.AddNestedTransactionsSupport.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:WindowFunctionsSupport"] = _extension.AddWindowFunctionsSupport.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:TenantDatabaseSupport"] = _extension.AddTenantDatabaseSupport.ToString(CultureInfo.InvariantCulture);
         debugInfo["Thinktecture:EvaluatableExpressionFilterPlugins"] = String.Join(", ", _extension._evaluatableExpressionFilterPlugins.Select(t => t.ShortDisplayName()));
         debugInfo["Thinktecture:ServiceDescriptors"] = String.Join(", ", _extension._serviceDescriptors);
         debugInfo["Thinktecture:StringBuilderPool:InitialCapacity"] = String.Join(", ", _extension._stringBuilderPolicy.InitialCapacity);
         debugInfo["Thinktecture:StringBuilderPool:MaximumRetainedCapacity"] = String.Join(", ", _extension._stringBuilderPolicy.MaximumRetainedCapacity);
      }
   }
}
