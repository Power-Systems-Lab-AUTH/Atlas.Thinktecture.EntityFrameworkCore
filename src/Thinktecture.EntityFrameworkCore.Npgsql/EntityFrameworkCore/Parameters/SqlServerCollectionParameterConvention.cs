using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Thinktecture.Internal;

namespace Thinktecture.EntityFrameworkCore.Parameters;

internal class NpgsqlCollectionParameterConvention : IModelInitializedConvention
{
   public static readonly IModelInitializedConvention Instance = new NpgsqlCollectionParameterConvention();

   public void ProcessModelInitialized(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
   {
      AddScalarCollectionParameter<int>(modelBuilder);
      AddScalarCollectionParameter<int?>(modelBuilder);
      AddScalarCollectionParameter<long>(modelBuilder);
      AddScalarCollectionParameter<long?>(modelBuilder);
      AddScalarCollectionParameter<DateTime>(modelBuilder);
      AddScalarCollectionParameter<DateTime?>(modelBuilder);
      AddScalarCollectionParameter<Guid>(modelBuilder);
      AddScalarCollectionParameter<Guid?>(modelBuilder);
      AddScalarCollectionParameter<bool>(modelBuilder);
      AddScalarCollectionParameter<bool?>(modelBuilder);
      AddScalarCollectionParameter<byte>(modelBuilder);
      AddScalarCollectionParameter<byte?>(modelBuilder);
      AddScalarCollectionParameter<double>(modelBuilder);
      AddScalarCollectionParameter<double?>(modelBuilder);
      AddScalarCollectionParameter<DateTimeOffset>(modelBuilder);
      AddScalarCollectionParameter<DateTimeOffset?>(modelBuilder);
      AddScalarCollectionParameter<short>(modelBuilder);
      AddScalarCollectionParameter<short?>(modelBuilder);
      AddScalarCollectionParameter<float>(modelBuilder);
      AddScalarCollectionParameter<float?>(modelBuilder);
      AddScalarCollectionParameter<decimal>(modelBuilder);
      AddScalarCollectionParameter<decimal?>(modelBuilder);
      AddScalarCollectionParameter<TimeSpan>(modelBuilder);
      AddScalarCollectionParameter<TimeSpan?>(modelBuilder);
      AddScalarCollectionParameter<string>(modelBuilder);
   }

   private static void AddScalarCollectionParameter<TColumn1>(IConventionModelBuilder modelBuilder)
   {
      var builder = modelBuilder.SharedTypeEntity(EntityNameProvider.GetCollectionParameterName(typeof(TColumn1), true),
                                                  typeof(ScalarCollectionParameter<TColumn1>),
                                                  fromDataAnnotation: true);

      if (builder is null)
         return;

      builder.ToTable(typeof(ScalarCollectionParameter<TColumn1>).ShortDisplayName());
      builder.HasNoKey();
      builder.ExcludeTableFromMigrations(true);
   }
}
