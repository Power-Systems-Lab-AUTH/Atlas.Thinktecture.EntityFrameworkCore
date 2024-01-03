using Thinktecture.EntityFrameworkCore.BulkOperations;
using Thinktecture.TestDatabaseContext;

namespace Thinktecture.Extensions.DbContextExtensionsTests;

// ReSharper disable once InconsistentNaming
public class BulkInsertOrUpdateAsync : IntegrationTestsBase
{
   /// <inheritdoc />
   public BulkInsertOrUpdateAsync(ITestOutputHelper testOutputHelper, SqlServerFixture sqlServerFixture)
      : base(testOutputHelper, sqlServerFixture)
   {
   }

   [Fact]
   public async Task Should_insert_non_existing_entities()
   {
      var testEntity = new TestEntity
                       {
                          Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                          Name = "Name",
                          RequiredName = "RequiredName",
                          Count = 42
                       };

      var affectedRows = await ActDbContext.BulkInsertOrUpdateAsync(new[] { testEntity });

      affectedRows.Should().Be(1);

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[]
                                             {
                                                new TestEntity
                                                {
                                                   Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                                                   Name = "Name",
                                                   RequiredName = "RequiredName",
                                                   Count = 42
                                                }
                                             });
   }

   [Fact]
   public async Task Should_update_existing_entities()
   {
      var testEntity = new TestEntity
                       {
                          Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                          Name = "Name",
                          RequiredName = "RequiredName",
                          Count = 42
                       };
      ArrangeDbContext.Add(testEntity);
      await ArrangeDbContext.SaveChangesAsync();

      testEntity.Name = "changed";
      testEntity.Count = 43;

      var affectedRows = await ActDbContext.BulkInsertOrUpdateAsync(new[] { testEntity });

      affectedRows.Should().Be(1);

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[]
                                             {
                                                new TestEntity
                                                {
                                                   Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                                                   Name = "changed",
                                                   RequiredName = "RequiredName",
                                                   Count = 43
                                                }
                                             });
   }

   [Fact]
   public async Task Should_insert_and_update_specified_properties_only()
   {
      var existingEntity = new TestEntity
                           {
                              Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                              Name = "Name",
                              RequiredName = "RequiredName",
                              Count = 42
                           };
      ArrangeDbContext.Add(existingEntity);
      await ArrangeDbContext.SaveChangesAsync();

      var newEntity = new TestEntity
                      {
                         Id = new Guid("3AA6D70D-C619-4EB5-9819-8030506EA637"),
                         Name = "new",
                         RequiredName = "RequiredName",
                         Count = 1
                      };

      existingEntity.Name = "changed";
      existingEntity.Count = 43;

      var affectedRows = await ActDbContext.BulkInsertOrUpdateAsync(new[] { existingEntity, newEntity },
                                                                    new SqlServerBulkInsertOrUpdateOptions
                                                                    {
                                                                       PropertiesToInsert = IEntityPropertiesProvider.Include(TestEntity.GetRequiredProperties()),
                                                                       PropertiesToUpdate = IEntityPropertiesProvider.Include<TestEntity>(entity => entity.Name)
                                                                    }
                                                                   );

      affectedRows.Should().Be(2);

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[]
                                             {
                                                new TestEntity
                                                {
                                                   Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                                                   Name = "changed",
                                                   RequiredName = "RequiredName",
                                                   Count = 42
                                                },
                                                new TestEntity
                                                {
                                                   Id = new Guid("3AA6D70D-C619-4EB5-9819-8030506EA637"),
                                                   Name = null, // is not a required property
                                                   RequiredName = "RequiredName",
                                                   Count = 1
                                                }
                                             });
   }

   [Fact]
   public async Task Should_match_on_provided_properties()
   {
      var entity_1 = new TestEntity
                     {
                        Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                        Name = "Name", // matching criteria
                        RequiredName = "RequiredName",
                        Count = 42
                     };
      var entity_2 = new TestEntity
                     {
                        Id = new Guid("3AA6D70D-C619-4EB5-9819-8030506EA637"),
                        Name = "other",
                        RequiredName = "RequiredName",
                        Count = 1
                     };
      ArrangeDbContext.AddRange(entity_1, entity_2);
      await ArrangeDbContext.SaveChangesAsync();

      var testEntity = new TestEntity
                       {
                          Id = entity_2.Id,
                          Name = entity_1.Name, // matching criteria
                          RequiredName = "RequiredName",
                          Count = 100
                       };

      var affectedRows = await ActDbContext.BulkInsertOrUpdateAsync(new[] { testEntity },
                                                                    propertiesToUpdate: entity => entity.Count,
                                                                    propertiesToMatchOn: entity => entity.Name);

      affectedRows.Should().Be(1);

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[]
                                             {
                                                new TestEntity
                                                {
                                                   Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                                                   Name = "Name",
                                                   RequiredName = "RequiredName",
                                                   Count = 100 // the only updated value
                                                },
                                                new TestEntity
                                                {
                                                   Id = new Guid("3AA6D70D-C619-4EB5-9819-8030506EA637"),
                                                   Name = "other",
                                                   RequiredName = "RequiredName",
                                                   Count = 1
                                                }
                                             });
   }

   [Fact]
   public async Task Should_skip_update_if_no_properties_to_update()
   {
      var existingEntity = new TestEntity
                           {
                              Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                              Name = "Name",
                              RequiredName = "RequiredName",
                              Count = 42
                           };
      ArrangeDbContext.Add(existingEntity);
      await ArrangeDbContext.SaveChangesAsync();

      var newEntity = new TestEntity
                      {
                         Id = new Guid("3AA6D70D-C619-4EB5-9819-8030506EA637"),
                         Name = "new",
                         RequiredName = "RequiredName",
                         Count = 1
                      };

      existingEntity.Name = "changed";
      existingEntity.Count = 43;

      var affectedRows = await ActDbContext.BulkInsertOrUpdateAsync(new[] { existingEntity, newEntity },
                                                                    new SqlServerBulkInsertOrUpdateOptions
                                                                    {
                                                                       PropertiesToInsert = IEntityPropertiesProvider.Include(TestEntity.GetRequiredProperties()),
                                                                       PropertiesToUpdate = IEntityPropertiesProvider.Empty
                                                                    }
                                                                   );

      affectedRows.Should().Be(1);

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[]
                                             {
                                                new TestEntity
                                                {
                                                   Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                                                   Name = "Name",
                                                   RequiredName = "RequiredName",
                                                   Count = 42
                                                },
                                                new TestEntity
                                                {
                                                   Id = new Guid("3AA6D70D-C619-4EB5-9819-8030506EA637"),
                                                   Name = null, // is not a required property
                                                   RequiredName = "RequiredName",
                                                   Count = 1
                                                }
                                             });
   }

   [Fact]
   public async Task Should_skip_update_if_provided_key_properties_only()
   {
      var existingEntity = new TestEntity
                           {
                              Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                              Name = "Name",
                              RequiredName = "RequiredName",
                              Count = 42
                           };
      ArrangeDbContext.Add(existingEntity);
      await ArrangeDbContext.SaveChangesAsync();

      var newEntity = new TestEntity
                      {
                         Id = new Guid("3AA6D70D-C619-4EB5-9819-8030506EA637"),
                         Name = "new",
                         RequiredName = "RequiredName",
                         Count = 1
                      };

      existingEntity.Name = "changed";
      existingEntity.Count = 43;

      var affectedRows = await ActDbContext.BulkInsertOrUpdateAsync(new[] { existingEntity, newEntity },
                                                                    new SqlServerBulkInsertOrUpdateOptions
                                                                    {
                                                                       PropertiesToInsert = IEntityPropertiesProvider.Include(TestEntity.GetRequiredProperties()),
                                                                       PropertiesToUpdate = IEntityPropertiesProvider.Include<TestEntity>(entity => entity.Id)
                                                                    }
                                                                   );

      affectedRows.Should().Be(1);

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[]
                                             {
                                                new TestEntity
                                                {
                                                   Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                                                   Name = "Name",
                                                   RequiredName = "RequiredName",
                                                   Count = 42
                                                },
                                                new TestEntity
                                                {
                                                   Id = new Guid("3AA6D70D-C619-4EB5-9819-8030506EA637"),
                                                   Name = null, // is not a required property
                                                   RequiredName = "RequiredName",
                                                   Count = 1
                                                }
                                             });
   }

   [Fact]
   public async Task Should_insert_and_update_TestEntity_with_ComplexType()
   {
      // Arrange
      var testEntity_1 = new TestEntityWithComplexType(new Guid("54FF93FC-6BE9-4F19-A52E-E517CA9FEAA7"),
                                                       new BoundaryValueObject(2, 5));

      ArrangeDbContext.Add(testEntity_1);
      await ArrangeDbContext.SaveChangesAsync();

      // Act
      testEntity_1.Boundary = new BoundaryValueObject(10, 20);
      var testEntity_2 = new TestEntityWithComplexType(new Guid("67A9500B-CF51-4A39-8C89-F2EBF7EDE84D"),
                                                       new BoundaryValueObject(3, 4));

      await ActDbContext.BulkInsertOrUpdateAsync(new[] { testEntity_1, testEntity_2 });

      var loadedEntities = await AssertDbContext.TestEntities_with_ComplexType.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[] { testEntity_1, testEntity_2 });
   }
}
