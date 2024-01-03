using Microsoft.EntityFrameworkCore.Query;
using Thinktecture.EntityFrameworkCore.Infrastructure;

namespace Thinktecture.EntityFrameworkCore.Query.ExpressionTranslators;

/// <summary>
/// Plugin registering method translators.
/// </summary>
public sealed class NpgsqlMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
{
   /// <inheritdoc />
   public IEnumerable<IMethodCallTranslator> Translators { get; }

   /// <summary>
   /// Initializes new instance of <see cref="NpgsqlMethodCallTranslatorPlugin"/>.
   /// </summary>
   public NpgsqlMethodCallTranslatorPlugin(RelationalDbContextOptionsExtensionOptions options, ISqlExpressionFactory sqlExpressionFactory)
   {
      ArgumentNullException.ThrowIfNull(options);

      var translators = new List<IMethodCallTranslator>();

      if (options.WindowFunctionsSupportEnabled)
         translators.Add(new NpgsqlDbFunctionsTranslator(sqlExpressionFactory));

      Translators = translators;
   }
}
