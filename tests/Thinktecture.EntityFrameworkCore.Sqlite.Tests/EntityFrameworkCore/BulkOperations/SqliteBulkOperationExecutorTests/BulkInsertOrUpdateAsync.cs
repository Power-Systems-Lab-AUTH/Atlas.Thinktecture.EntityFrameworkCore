using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Thinktecture.TestDatabaseContext;

namespace Thinktecture.EntityFrameworkCore.BulkOperations.SqliteBulkOperationExecutorTests;

// ReSharper disable once InconsistentNaming
public class BulkInsertOrUpdateAsync : IntegrationTestsBase
{
   private SqliteBulkOperationExecutor? _sut;

   private SqliteBulkOperationExecutor SUT => _sut ??= ActDbContext.GetService<SqliteBulkOperationExecutor>();

   public BulkInsertOrUpdateAsync(ITestOutputHelper testOutputHelper, DbContextProviderFactoryFixture providerFactoryFixture)
      : base(testOutputHelper, providerFactoryFixture)
   {
   }

   [Fact]
   public async Task Should_throw_when_entity_has_no_key()
   {
      await SUT.Invoking(sut => sut.BulkInsertOrUpdateAsync(new List<KeylessTestEntity> { new() }, new SqliteBulkInsertOrUpdateOptions()))
               .Should().ThrowAsync<InvalidOperationException>()
               .WithMessage("The entity 'Thinktecture.TestDatabaseContext.KeylessTestEntity' has no primary key. Please provide key properties to perform JOIN/match on.");
   }

   [Fact]
   public async Task Should_not_throw_if_entities_is_empty()
   {
      var affectedRows = await SUT.BulkInsertOrUpdateAsync(new List<TestEntity>(), new SqliteBulkInsertOrUpdateOptions());

      affectedRows.Should().Be(0);
   }

   [Fact]
   public async Task Should_insert_column_with_converter()
   {
      var existingEntity = new TestEntity { Id = new Guid("79DA4171-C90B-4A5D-B0B5-D0A1E1BDF966"), RequiredName = "RequiredName" };
      ArrangeDbContext.Add(existingEntity);
      await ArrangeDbContext.SaveChangesAsync();

      existingEntity.ConvertibleClass = new ConvertibleClass(43);
      var newEntity = new TestEntity
                      {
                         Id = new Guid("3DAEA618-B732-4BCA-A5A1-D1E075022DEC"),
                         RequiredName = "RequiredName",
                         ConvertibleClass = new ConvertibleClass(42)
                      };

      var affectedRows = await SUT.BulkInsertOrUpdateAsync(new[] { existingEntity, newEntity }, new SqliteBulkInsertOrUpdateOptions());

      affectedRows.Should().Be(2);

      var entities = AssertDbContext.TestEntities.ToList();
      entities.Should().BeEquivalentTo(new[]
                                       {
                                          new TestEntity { Id = new Guid("79DA4171-C90B-4A5D-B0B5-D0A1E1BDF966"), RequiredName = "RequiredName", ConvertibleClass = new ConvertibleClass(43) },
                                          new TestEntity { Id = new Guid("3DAEA618-B732-4BCA-A5A1-D1E075022DEC"), RequiredName = "RequiredName", ConvertibleClass = new ConvertibleClass(42) }
                                       });
   }

   [Fact]
   public async Task Should_insert_entities()
   {
      var testEntity = new TestEntity
                       {
                          Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                          Name = "Name",
                          RequiredName = "RequiredName",
                          Count = 42
                       };

      await SUT.BulkInsertOrUpdateAsync(new[] { testEntity }, new SqliteBulkInsertOrUpdateOptions());

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().HaveCount(1)
                    .And.Subject.First()
                    .Should().BeEquivalentTo(new TestEntity
                                             {
                                                Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                                                Name = "Name",
                                                RequiredName = "RequiredName",
                                                Count = 42
                                             });
   }

   [Fact]
   public async Task Should_insert_private_property()
   {
      var existingEntity = new TestEntity { Id = new Guid("7C200656-E633-4F93-9F73-C5C7628196DC"), RequiredName = "RequiredName" };
      ArrangeDbContext.Add(existingEntity);
      await ArrangeDbContext.SaveChangesAsync();

      existingEntity.SetPrivateField(1);

      var newEntity = new TestEntity { Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"), RequiredName = "RequiredName" };
      newEntity.SetPrivateField(3);

      var affectedRows = await SUT.BulkInsertOrUpdateAsync(new[] { newEntity, existingEntity }, new SqliteBulkInsertOrUpdateOptions());

      affectedRows.Should().Be(2);

      var expectedExistingEntity = new TestEntity { Id = new Guid("7C200656-E633-4F93-9F73-C5C7628196DC"), RequiredName = "RequiredName" };
      expectedExistingEntity.SetPrivateField(1);

      var expectedNewEntity = new TestEntity { Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"), RequiredName = "RequiredName" };
      expectedNewEntity.SetPrivateField(3);

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[] { expectedNewEntity, expectedExistingEntity });
   }

   [Fact]
   public async Task Should_insert_shadow_properties()
   {
      var existingEntity = new TestEntityWithShadowProperties { Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866") };
      ArrangeDbContext.Add(existingEntity);
      await ArrangeDbContext.SaveChangesAsync();

      ActDbContext.Entry(existingEntity).Property("ShadowStringProperty").CurrentValue = "value1";
      ActDbContext.Entry(existingEntity).Property("ShadowIntProperty").CurrentValue = 42;

      var newEntity = new TestEntityWithShadowProperties { Id = new Guid("884BB11B-088D-4BA5-B75D-C7F36B88378B") };
      ActDbContext.Entry(newEntity).Property("ShadowStringProperty").CurrentValue = "value2";
      ActDbContext.Entry(newEntity).Property("ShadowIntProperty").CurrentValue = 43;

      var affectedRows = await SUT.BulkInsertOrUpdateAsync(new[] { newEntity, existingEntity }, new SqliteBulkInsertOrUpdateOptions());

      affectedRows.Should().Be(2);

      var loadedEntities = await AssertDbContext.TestEntitiesWithShadowProperties.ToListAsync();
      loadedEntities.Should().HaveCount(2);

      var existingEntry = AssertDbContext.Entry(loadedEntities.Single(e => e.Id == new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866")));
      existingEntry.Property("ShadowStringProperty").CurrentValue.Should().Be("value1");
      existingEntry.Property("ShadowIntProperty").CurrentValue.Should().Be(42);

      var newEntry = AssertDbContext.Entry(loadedEntities.Single(e => e.Id == new Guid("884BB11B-088D-4BA5-B75D-C7F36B88378B")));
      newEntry.Property("ShadowStringProperty").CurrentValue.Should().Be("value2");
      newEntry.Property("ShadowIntProperty").CurrentValue.Should().Be(43);
   }

   [Fact]
   public async Task Should_insert_and_update_entities_with_auto_increment_column_having_property_selector()
   {
      var existingEntity = new TestEntityWithAutoIncrement { Name = "original value" };
      ArrangeDbContext.Add(existingEntity);
      await ArrangeDbContext.SaveChangesAsync();

      existingEntity.Name = "Name";

      var newEntity = new TestEntityWithAutoIncrement { Name = "New Entity" };
      var testEntities = new[] { existingEntity, newEntity };

      var affectedRows = await SUT.BulkInsertOrUpdateAsync(testEntities, new SqliteBulkInsertOrUpdateOptions
                                                                         {
                                                                            PropertiesToInsert = IEntityPropertiesProvider.Include<TestEntityWithAutoIncrement>(entity => entity.Name)
                                                                         });

      affectedRows.Should().Be(2);

      var loadedEntities = await AssertDbContext.TestEntitiesWithAutoIncrement.ToListAsync();
      loadedEntities.Should().HaveCount(2);
      loadedEntities.Should().AllSatisfy(e => e.Id.Should().NotBe(0));
      loadedEntities.Should().BeEquivalentTo(new[]
                                             {
                                                existingEntity,
                                                new TestEntityWithAutoIncrement
                                                {
                                                   Id = loadedEntities.Single(e => e.Id != existingEntity.Id).Id,
                                                   Name = "New Entity"
                                                }
                                             });
   }

   [Fact]
   public async Task Should_insert_specified_properties_only()
   {
      var testEntity = new TestEntity
                       {
                          Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                          Name = "Name",
                          RequiredName = "RequiredName",
                          Count = 42,
                          PropertyWithBackingField = 7
                       };
      testEntity.SetPrivateField(3);
      var testEntities = new[] { testEntity };

      await SUT.BulkInsertOrUpdateAsync(testEntities,
                                        new SqliteBulkInsertOrUpdateOptions
                                        {
                                           PropertiesToInsert = IEntityPropertiesProvider.Include(TestEntity.GetRequiredProperties())
                                        });

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().HaveCount(1);
      var loadedEntity = loadedEntities[0];
      loadedEntity.Should().BeEquivalentTo(new TestEntity
                                           {
                                              Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                                              RequiredName = "RequiredName",
                                              Count = 42,
                                              PropertyWithBackingField = 7
                                           });
      loadedEntity.GetPrivateField().Should().Be(3);
   }

   [Fact]
   public async Task Should_not_throw_if_key_property_is_not_present_in_PropertiesToUpdate()
   {
      var propertiesProvider = IEntityPropertiesProvider.Include<TestEntity>(entity => new { entity.Name });

      var affectedRows = await SUT.BulkInsertOrUpdateAsync(new List<TestEntity>(), new SqliteBulkInsertOrUpdateOptions
                                                                                   {
                                                                                      PropertiesToUpdate = propertiesProvider
                                                                                   });

      affectedRows.Should().Be(0);
   }

   [Fact]
   public async Task Should_update_entities()
   {
      var entity_1 = new TestEntity { Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"), RequiredName = "RequiredName" };
      var entity_2 = new TestEntity { Id = new Guid("8AF163D7-D316-4B2D-A62F-6326A80C8BEE"), RequiredName = "RequiredName" };
      ArrangeDbContext.AddRange(entity_1, entity_2);
      await ArrangeDbContext.SaveChangesAsync();

      entity_1.Name = "Name";
      entity_1.Count = 1;
      entity_2.Name = "OtherName";
      entity_2.Count = 2;

      var affectedRows = await SUT.BulkInsertOrUpdateAsync(new[] { entity_1, entity_2 }, new SqliteBulkInsertOrUpdateOptions());

      affectedRows.Should().Be(2);

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[] { entity_1, entity_2 });
   }

   [Fact]
   public async Task Should_update_entities_based_on_non_pk_property()
   {
      var keyProperties = IEntityPropertiesProvider.Include<TestEntity>(e => e.Name);

      await SUT.Awaiting(sut => sut.BulkInsertOrUpdateAsync(new TestEntity[0], new SqliteBulkInsertOrUpdateOptions { KeyProperties = keyProperties }))
               .Should().ThrowAsync<InvalidOperationException>().WithMessage("Error during bulk operation on table '\"TestEntities\"'. See inner exception for more details.")
               .WithInnerException<InvalidOperationException, SqliteException>().WithMessage("SQLite Error 1: 'ON CONFLICT clause does not match any PRIMARY KEY or UNIQUE constraint'.");
   }

   [Fact]
   public async Task Should_update_provided_entity_only()
   {
      var entity_1 = new TestEntity { Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"), RequiredName = "RequiredName" };
      var entity_2 = new TestEntity { Id = new Guid("8AF163D7-D316-4B2D-A62F-6326A80C8BEE"), RequiredName = "RequiredName" };
      ArrangeDbContext.AddRange(entity_1, entity_2);
      await ArrangeDbContext.SaveChangesAsync();

      entity_1.Name = "Name";
      entity_1.Count = 1;
      entity_2.Name = "OtherName";
      entity_2.Count = 2;

      var affectedRows = await SUT.BulkInsertOrUpdateAsync(new[] { entity_1 }, new SqliteBulkInsertOrUpdateOptions());

      affectedRows.Should().Be(1);

      entity_2.Name = default;
      entity_2.Count = default;

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[] { entity_1, entity_2 });
   }

   [Fact]
   public async Task Should_write_not_nullable_structs_as_is_despite_sql_default_value()
   {
      var entity = new TestEntityWithSqlDefaultValues
                   {
                      Id = new Guid("53A5EC9B-BB8D-4B9D-9136-68C011934B63"),
                      Int = 1,
                      String = "value",
                      NullableInt = 2,
                      NullableString = "otherValue"
                   };
      ArrangeDbContext.Add(entity);
      await ArrangeDbContext.SaveChangesAsync();

      entity.Int = 0;
      entity.String = "other value";
      entity.NullableInt = null;
      entity.NullableString = null;

      var affectedRows = await SUT.BulkInsertOrUpdateAsync(new[] { entity }, new SqliteBulkInsertOrUpdateOptions());

      affectedRows.Should().Be(1);

      var loadedEntity = await AssertDbContext.TestEntitiesWithDefaultValues.FirstOrDefaultAsync();
      loadedEntity.Should().BeEquivalentTo(new TestEntityWithSqlDefaultValues
                                           {
                                              Id = new Guid("53A5EC9B-BB8D-4B9D-9136-68C011934B63"),
                                              Int = 0,            // persisted as-is
                                              NullableInt = null, // persisted as-is
                                              String = "other value",
                                              NullableString = null // persisted as-is
                                           });
   }

   [Fact]
   public async Task Should_update_specified_properties_only()
   {
      var entity = new TestEntity { Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"), Name = "original value", RequiredName = "RequiredName" };
      ArrangeDbContext.Add(entity);
      await ArrangeDbContext.SaveChangesAsync();

      entity.Name = "Name"; // this would not be updated
      entity.Count = 42;
      entity.PropertyWithBackingField = 7;
      entity.SetPrivateField(3);

      var affectedRows = await SUT.BulkInsertOrUpdateAsync(new[] { entity },
                                                           new SqliteBulkInsertOrUpdateOptions
                                                           {
                                                              PropertiesToUpdate = IEntityPropertiesProvider.Include(TestEntity.GetRequiredProperties())
                                                           });

      affectedRows.Should().Be(1);

      var loadedEntities = await AssertDbContext.TestEntities.ToListAsync();
      loadedEntities.Should().HaveCount(1);
      var loadedEntity = loadedEntities[0];
      loadedEntity.Should().BeEquivalentTo(new TestEntity
                                           {
                                              Id = new Guid("40B5CA93-5C02-48AD-B8A1-12BC13313866"),
                                              Count = 42,
                                              PropertyWithBackingField = 7,
                                              Name = "original value",
                                              RequiredName = "RequiredName"
                                           });
      loadedEntity.GetPrivateField().Should().Be(3);
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
                                                                    new SqliteBulkInsertOrUpdateOptions
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
                                                                    new SqliteBulkInsertOrUpdateOptions
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

      await SUT.BulkInsertOrUpdateAsync(new[] { testEntity_1, testEntity_2 }, new SqliteBulkInsertOrUpdateOptions());

      var loadedEntities = await AssertDbContext.TestEntities_with_ComplexType.ToListAsync();
      loadedEntities.Should().BeEquivalentTo(new[] { testEntity_1, testEntity_2 });
   }
}
