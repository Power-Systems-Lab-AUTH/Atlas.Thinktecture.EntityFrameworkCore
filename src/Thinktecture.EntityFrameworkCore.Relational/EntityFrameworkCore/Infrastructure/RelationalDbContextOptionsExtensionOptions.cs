using Microsoft.EntityFrameworkCore.Infrastructure;
using Thinktecture.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Thinktecture.EntityFrameworkCore.Infrastructure;

/// <summary>
/// Options for the <see cref="RelationalMethodCallTranslatorPlugin"/>.
/// </summary>
public class RelationalDbContextOptionsExtensionOptions : ISingletonOptions
{
   /// <summary>
   /// Indication whether the support for windows functions is enabled or not.
   /// </summary>
   public bool WindowFunctionsSupportEnabled { get; private set; }

   /// <summary>
   /// Indication whether the 'tenant database support' is enabled or not.
   /// </summary>
   public bool TenantDatabaseSupportEnabled { get; private set; }

   /// <inheritdoc />
   public void Initialize(IDbContextOptions options)
   {
      var extension = GetExtension(options);

      WindowFunctionsSupportEnabled = extension.AddWindowFunctionsSupport;
      TenantDatabaseSupportEnabled = extension.AddTenantDatabaseSupport;
   }

   /// <inheritdoc />
   public void Validate(IDbContextOptions options)
   {
      var extension = GetExtension(options);

      if (extension.AddWindowFunctionsSupport != WindowFunctionsSupportEnabled)
         throw new InvalidOperationException($"The setting '{nameof(RelationalDbContextOptionsExtension.AddWindowFunctionsSupport)}' has been changed.");

      if (extension.AddTenantDatabaseSupport != TenantDatabaseSupportEnabled)
         throw new InvalidOperationException($"The setting '{nameof(RelationalDbContextOptionsExtension.AddTenantDatabaseSupport)}' has been changed.");
   }

   private static RelationalDbContextOptionsExtension GetExtension(IDbContextOptions options)
   {
      return options.FindExtension<RelationalDbContextOptionsExtension>()
             ?? throw new InvalidOperationException($"{nameof(RelationalDbContextOptionsExtension)} not found in current '{nameof(IDbContextOptions)}'.");
   }
}
