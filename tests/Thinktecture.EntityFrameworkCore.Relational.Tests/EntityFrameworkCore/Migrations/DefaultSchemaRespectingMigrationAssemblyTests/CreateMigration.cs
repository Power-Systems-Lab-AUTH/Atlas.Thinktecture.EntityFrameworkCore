using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Thinktecture.TestDatabaseContext;

namespace Thinktecture.EntityFrameworkCore.Migrations.DefaultSchemaRespectingMigrationAssemblyTests;

public class CreateMigration : DefaultSchemaRespectingMigrationAssemblyTestsBase
{
   public CreateMigration(ITestOutputHelper testOutputHelper)
      : base(testOutputHelper)
   {
   }

   [Fact]
   public void Should_throw_when_schema_type_is_null()
   {
      CurrentCtxMock.Context.Returns(CreateContextWithSchema("Schema1"));

      // ReSharper disable once AssignNullToNotNullAttribute
      SUT.Invoking(sut => sut.CreateMigration(null!, "DummyProvider"))
         .Should().Throw<ArgumentNullException>();
   }

   [Fact]
   public void Should_throw_when_active_provider_is_null()
   {
      CurrentCtxMock.Context.Returns(CreateContextWithSchema("Schema1"));

      // ReSharper disable once AssignNullToNotNullAttribute
      SUT.Invoking(sut => sut.CreateMigration(typeof(MigrationWithSchema).GetTypeInfo(), null!))
         .Should().Throw<ArgumentNullException>();
   }

   [Fact]
   public void Should_create_schema_aware_migration_having_schema_aware_ctx()
   {
      CurrentCtxMock.Context.Returns(CreateContextWithSchema("Schema1"));

      var migration = SUT.CreateMigration(typeof(MigrationWithSchema).GetTypeInfo(), "DummyProvider");

      migration.Should().NotBeNull();
      migration.Should().BeOfType<MigrationWithSchema>();
   }

   [Fact]
   public void Should_set_schema_of_schema_aware_migration_having_schema_aware_ctx()
   {
      CurrentCtxMock.Context.Returns(CreateContextWithSchema("Schema1"));
      SchemaSetterMock.When(s => s.SetSchema(Arg.Any<IReadOnlyList<MigrationOperation>>(), Arg.Any<string>()))
                      .Do(x => new MigrationOperationSchemaSetter().SetSchema((IReadOnlyList<MigrationOperation>)x[0], (string)x[1]));

      var migration = SUT.CreateMigration(typeof(MigrationWithSchema).GetTypeInfo(), "DummyProvider");

      VerifySchema(migration, "Schema1");
   }

   [Fact]
   public void Should_create_migration_having_schema_aware_ctx()
   {
      CurrentCtxMock.Context.Returns(CreateContextWithSchema("Schema1"));
      var migration = new MigrationWithoutSchema { ActiveProvider = "DummyProvider" };

      var createMigration = SUT.CreateMigration(typeof(MigrationWithoutSchema).GetTypeInfo(), "DummyProvider");

      migration.Should().NotBeNull();
      migration.Should().BeOfType<MigrationWithoutSchema>();

      createMigration.Should().BeEquivalentTo(migration, options => options.Excluding(m => m.TargetModel.ModelId));
   }

   [Fact]
   public void Should_set_schema_of_migration_having_schema_aware_ctx()
   {
      CurrentCtxMock.Context.Returns(CreateContextWithSchema("Schema1"));
      SchemaSetterMock.When(s => s.SetSchema(Arg.Any<IReadOnlyList<MigrationOperation>>(), Arg.Any<string>()))
                      .Do(x => new MigrationOperationSchemaSetter().SetSchema((IReadOnlyList<MigrationOperation>)x[0], (string)x[1]));

      var migration = SUT.CreateMigration(typeof(MigrationWithoutSchema).GetTypeInfo(), "DummyProvider");

      VerifySchema(migration, "Schema1");
   }

   [Fact]
   public void Should_throw_when_creating_schema_aware_migration_having_schema_unaware_ctx()
   {
      CurrentCtxMock.Context.Returns(CreateContextWithoutSchema());

      SUT.Invoking(sut => sut.CreateMigration(typeof(MigrationWithSchema).GetTypeInfo(), "DummyProvider"))
         .Should().Throw<ArgumentException>().WithMessage($"For instantiation of default schema respecting migration of type '{nameof(MigrationWithSchema)}' the database context of type '{nameof(DbContextWithoutSchema)}' has to implement the interface '{nameof(IDbDefaultSchema)}'. (Parameter 'migrationClass')");
   }

   [Fact]
   public void Should_create_schema_unaware_migration_having_schema_unaware_ctx()
   {
      CurrentCtxMock.Context.Returns(CreateContextWithoutSchema());
      var migration = new MigrationWithoutSchema { ActiveProvider = "DummyProvider" };

      var createMigration = SUT.CreateMigration(typeof(MigrationWithoutSchema).GetTypeInfo(), "DummyProvider");

      createMigration.Should().BeEquivalentTo(migration, options => options.Excluding(m => m.TargetModel.ModelId));
   }

   [Fact]
   public void Should_not_set_schema_on_schema_unaware_migration_having_schema_unaware_ctx()
   {
      CurrentCtxMock.Context.Returns(CreateContextWithoutSchema());

      var migration = SUT.CreateMigration(typeof(MigrationWithoutSchema).GetTypeInfo(), "DummyProvider");

      SchemaSetterMock.DidNotReceive().SetSchema(Arg.Any<IReadOnlyList<MigrationOperation>>(), Arg.Any<string>());
      migration.UpOperations[0].Should().BeOfType<AddColumnOperation>().Subject.Schema.Should().BeNull();
      migration.DownOperations[0].Should().BeOfType<DropColumnOperation>().Subject.Schema.Should().BeNull();
   }

   private void VerifySchema(Migration migration, string schema)
   {
      SchemaSetterMock.Received(1).SetSchema(migration.UpOperations, schema);
      SchemaSetterMock.Received(1).SetSchema(migration.DownOperations, schema);

      migration.UpOperations[0].Should().BeOfType<AddColumnOperation>().Subject.Schema.Should().Be(schema);
      migration.DownOperations[0].Should().BeOfType<DropColumnOperation>().Subject.Schema.Should().Be(schema);
   }
}
