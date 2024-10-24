﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thinktecture.TestDatabaseContext;

#nullable disable

namespace Thinktecture.Migrations
{
    [DbContext(typeof(TestDbContext))]
    [Migration("20231019191758_Add_NullableCount")]
    partial class Add_NullableCount
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.KeylessTestEntity", b =>
                {
                    b.Property<int>("IntColumn")
                        .HasColumnType("INTEGER");

                    b.ToTable("KeylessEntities");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.SqliteIndex", b =>
                {
                    b.Property<byte[]>("Name")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("Origin")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("Partial")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("Seq")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("Unique")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.ToTable((string)null);

                    b.ToView("pragma temp.index_list('<<table-name>>')", (string)null);
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.SqliteMaster", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long?>("Rootpage")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Sql")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tbl_Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.ToTable((string)null);

                    b.ToView("sqlite_temp_master", (string)null);
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.SqliteTableInfo", b =>
                {
                    b.Property<long>("CId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Dflt_Value")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long?>("NotNull")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("PK")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.ToTable((string)null);

                    b.ToView("PRAGMA_TABLE_INFO('<<table-name>>')", (string)null);
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ConvertibleClass")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Count")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("NullableCount")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<int>("PropertyWithBackingField")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RequiredName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("_privateField")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_TestEntities_Id");

                    b.HasIndex("ParentId");

                    b.ToTable("TestEntities");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntityWithAutoIncrement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntitiesWithAutoIncrement");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntityWithBaseClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntitiesWithBaseClass");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntityWithDotnetDefaultValues", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue(new Guid("0b151271-79bb-4f6c-b85f-e8f61300ff1b"));

                    b.Property<int>("Int")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(1);

                    b.Property<int?>("NullableInt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(2);

                    b.Property<string>("NullableString")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("4");

                    b.Property<string>("String")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("3");

                    b.HasKey("Id");

                    b.ToTable("TestEntitiesWithDotnetDefaultValues");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntityWithShadowProperties", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ShadowIntProperty")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ShadowStringProperty")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntitiesWithShadowProperties");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntityWithSqlDefaultValues", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("Int")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValueSql("1");

                    b.Property<int?>("NullableInt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValueSql("2");

                    b.Property<string>("NullableString")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("'4'");

                    b.Property<string>("String")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("'3'");

                    b.HasKey("Id");

                    b.ToTable("TestEntitiesWithDefaultValues");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_Inline", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_Inline");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_Inline_Inline", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_Inline_Inline");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_Inline_SeparateMany", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_Inline_SeparateMany");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_Inline_SeparateOne", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_Inline_SeparateOne");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateMany", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_SeparateMany");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateMany_Inline", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_SeparateMany_Inline");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateMany_SeparateMany", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_SeparateMany_SeparateMany");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateMany_SeparateOne", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_SeparateMany_SeparateOne");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateOne", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_SeparateOne");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateOne_Inline", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_SeparateOne_Inline");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateOne_SeparateMany", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_SeparateOne_SeparateMany");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateOne_SeparateOne", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TestEntities_Own_SeparateOne_SeparateOne");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity", b =>
                {
                    b.HasOne("Thinktecture.TestDatabaseContext.TestEntity", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_Inline", b =>
                {
                    b.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity", "InlineEntity", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_InlineId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_InlineId");

                            b1.ToTable("TestEntities_Own_Inline");

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_InlineId");
                        });

                    b.Navigation("InlineEntity")
                        .IsRequired();
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_Inline_Inline", b =>
                {
                    b.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity_Owns_Inline", "InlineEntity", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_Inline_InlineId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_Inline_InlineId");

                            b1.ToTable("TestEntities_Own_Inline_Inline");

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_Inline_InlineId");

                            b1.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity", "InlineEntity", b2 =>
                                {
                                    b2.Property<Guid>("OwnedEntity_Owns_InlineTestEntity_Owns_Inline_InlineId")
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("IntColumn")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("StringColumn")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("OwnedEntity_Owns_InlineTestEntity_Owns_Inline_InlineId");

                                    b2.ToTable("TestEntities_Own_Inline_Inline");

                                    b2.WithOwner()
                                        .HasForeignKey("OwnedEntity_Owns_InlineTestEntity_Owns_Inline_InlineId");
                                });

                            b1.Navigation("InlineEntity")
                                .IsRequired();
                        });

                    b.Navigation("InlineEntity")
                        .IsRequired();
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_Inline_SeparateMany", b =>
                {
                    b.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity_Owns_SeparateMany", "InlineEntity", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_Inline_SeparateManyId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_Inline_SeparateManyId");

                            b1.ToTable("TestEntities_Own_Inline_SeparateMany");

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_Inline_SeparateManyId");

                            b1.OwnsMany("Thinktecture.TestDatabaseContext.OwnedEntity", "SeparateEntities", b2 =>
                                {
                                    b2.Property<Guid>("OwnedEntity_Owns_SeparateManyTestEntity_Owns_Inline_SeparateManyId")
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("IntColumn")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("StringColumn")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("OwnedEntity_Owns_SeparateManyTestEntity_Owns_Inline_SeparateManyId", "Id");

                                    b2.ToTable("InlineEntities_SeparateMany", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("OwnedEntity_Owns_SeparateManyTestEntity_Owns_Inline_SeparateManyId");
                                });

                            b1.Navigation("SeparateEntities");
                        });

                    b.Navigation("InlineEntity")
                        .IsRequired();
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_Inline_SeparateOne", b =>
                {
                    b.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity_Owns_SeparateOne", "InlineEntity", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_Inline_SeparateOneId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_Inline_SeparateOneId");

                            b1.ToTable("TestEntities_Own_Inline_SeparateOne");

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_Inline_SeparateOneId");

                            b1.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity", "SeparateEntity", b2 =>
                                {
                                    b2.Property<Guid>("OwnedEntity_Owns_SeparateOneTestEntity_Owns_Inline_SeparateOneId")
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("IntColumn")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("StringColumn")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("OwnedEntity_Owns_SeparateOneTestEntity_Owns_Inline_SeparateOneId");

                                    b2.ToTable("InlineEntities_SeparateOne", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("OwnedEntity_Owns_SeparateOneTestEntity_Owns_Inline_SeparateOneId");
                                });

                            b1.Navigation("SeparateEntity")
                                .IsRequired();
                        });

                    b.Navigation("InlineEntity")
                        .IsRequired();
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateMany", b =>
                {
                    b.OwnsMany("Thinktecture.TestDatabaseContext.OwnedEntity", "SeparateEntities", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_SeparateManyId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Id")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_SeparateManyId", "Id");

                            b1.ToTable("SeparateEntitiesMany", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_SeparateManyId");
                        });

                    b.Navigation("SeparateEntities");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateMany_Inline", b =>
                {
                    b.OwnsMany("Thinktecture.TestDatabaseContext.OwnedEntity_Owns_Inline", "SeparateEntities", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_SeparateMany_InlineId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_SeparateMany_InlineId", "Id");

                            b1.ToTable("SeparateEntitiesMany_Inline", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_SeparateMany_InlineId");

                            b1.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity", "InlineEntity", b2 =>
                                {
                                    b2.Property<Guid>("OwnedEntity_Owns_InlineTestEntity_Owns_SeparateMany_InlineId")
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("OwnedEntity_Owns_InlineId")
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("IntColumn")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("StringColumn")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("OwnedEntity_Owns_InlineTestEntity_Owns_SeparateMany_InlineId", "OwnedEntity_Owns_InlineId");

                                    b2.ToTable("SeparateEntitiesMany_Inline");

                                    b2.WithOwner()
                                        .HasForeignKey("OwnedEntity_Owns_InlineTestEntity_Owns_SeparateMany_InlineId", "OwnedEntity_Owns_InlineId");
                                });

                            b1.Navigation("InlineEntity")
                                .IsRequired();
                        });

                    b.Navigation("SeparateEntities");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateMany_SeparateMany", b =>
                {
                    b.OwnsMany("Thinktecture.TestDatabaseContext.OwnedEntity_Owns_SeparateMany", "SeparateEntities", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_SeparateMany_SeparateManyId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_SeparateMany_SeparateManyId", "Id");

                            b1.ToTable("SeparateEntitiesMany_SeparateEntitiesMany", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_SeparateMany_SeparateManyId");

                            b1.OwnsMany("Thinktecture.TestDatabaseContext.OwnedEntity", "SeparateEntities", b2 =>
                                {
                                    b2.Property<Guid>("OwnedEntity_Owns_SeparateManyTestEntity_Owns_SeparateMany_SeparateManyId")
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("OwnedEntity_Owns_SeparateManyId")
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("IntColumn")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("StringColumn")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("OwnedEntity_Owns_SeparateManyTestEntity_Owns_SeparateMany_SeparateManyId", "OwnedEntity_Owns_SeparateManyId", "Id");

                                    b2.ToTable("SeparateEntitiesMany_SeparateEntitiesMany_Inner", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("OwnedEntity_Owns_SeparateManyTestEntity_Owns_SeparateMany_SeparateManyId", "OwnedEntity_Owns_SeparateManyId");
                                });

                            b1.Navigation("SeparateEntities");
                        });

                    b.Navigation("SeparateEntities");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateMany_SeparateOne", b =>
                {
                    b.OwnsMany("Thinktecture.TestDatabaseContext.OwnedEntity_Owns_SeparateOne", "SeparateEntities", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_SeparateMany_SeparateOneId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_SeparateMany_SeparateOneId", "Id");

                            b1.ToTable("SeparateEntitiesMany_SeparateEntitiesOne", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_SeparateMany_SeparateOneId");

                            b1.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity", "SeparateEntity", b2 =>
                                {
                                    b2.Property<Guid>("OwnedEntity_Owns_SeparateOneTestEntity_Owns_SeparateMany_SeparateOneId")
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("OwnedEntity_Owns_SeparateOneId")
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("IntColumn")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("StringColumn")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("OwnedEntity_Owns_SeparateOneTestEntity_Owns_SeparateMany_SeparateOneId", "OwnedEntity_Owns_SeparateOneId");

                                    b2.ToTable("SeparateEntitiesMany_SeparateEntitiesOne_Inner", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("OwnedEntity_Owns_SeparateOneTestEntity_Owns_SeparateMany_SeparateOneId", "OwnedEntity_Owns_SeparateOneId");
                                });

                            b1.Navigation("SeparateEntity")
                                .IsRequired();
                        });

                    b.Navigation("SeparateEntities");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateOne", b =>
                {
                    b.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity", "SeparateEntity", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_SeparateOneId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_SeparateOneId");

                            b1.ToTable("SeparateEntitiesOne", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_SeparateOneId");
                        });

                    b.Navigation("SeparateEntity");
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateOne_Inline", b =>
                {
                    b.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity_Owns_Inline", "SeparateEntity", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_SeparateOne_InlineId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_SeparateOne_InlineId");

                            b1.ToTable("SeparateEntitiesOne_Inline", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_SeparateOne_InlineId");

                            b1.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity", "InlineEntity", b2 =>
                                {
                                    b2.Property<Guid>("OwnedEntity_Owns_InlineTestEntity_Owns_SeparateOne_InlineId")
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("IntColumn")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("StringColumn")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("OwnedEntity_Owns_InlineTestEntity_Owns_SeparateOne_InlineId");

                                    b2.ToTable("SeparateEntitiesOne_Inline");

                                    b2.WithOwner()
                                        .HasForeignKey("OwnedEntity_Owns_InlineTestEntity_Owns_SeparateOne_InlineId");
                                });

                            b1.Navigation("InlineEntity")
                                .IsRequired();
                        });

                    b.Navigation("SeparateEntity")
                        .IsRequired();
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateOne_SeparateMany", b =>
                {
                    b.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity_Owns_SeparateMany", "SeparateEntity", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_SeparateOne_SeparateManyId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_SeparateOne_SeparateManyId");

                            b1.ToTable("SeparateEntitiesOne_SeparateMany", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_SeparateOne_SeparateManyId");

                            b1.OwnsMany("Thinktecture.TestDatabaseContext.OwnedEntity", "SeparateEntities", b2 =>
                                {
                                    b2.Property<Guid>("OwnedEntity_Owns_SeparateManyTestEntity_Owns_SeparateOne_SeparateManyId")
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("IntColumn")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("StringColumn")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("OwnedEntity_Owns_SeparateManyTestEntity_Owns_SeparateOne_SeparateManyId", "Id");

                                    b2.ToTable("SeparateEntitiesOne_SeparateMany_Inner", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("OwnedEntity_Owns_SeparateManyTestEntity_Owns_SeparateOne_SeparateManyId");
                                });

                            b1.Navigation("SeparateEntities");
                        });

                    b.Navigation("SeparateEntity")
                        .IsRequired();
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity_Owns_SeparateOne_SeparateOne", b =>
                {
                    b.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity_Owns_SeparateOne", "SeparateEntity", b1 =>
                        {
                            b1.Property<Guid>("TestEntity_Owns_SeparateOne_SeparateOneId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("IntColumn")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StringColumn")
                                .HasColumnType("TEXT");

                            b1.HasKey("TestEntity_Owns_SeparateOne_SeparateOneId");

                            b1.ToTable("SeparateEntitiesOne_SeparateOne", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("TestEntity_Owns_SeparateOne_SeparateOneId");

                            b1.OwnsOne("Thinktecture.TestDatabaseContext.OwnedEntity", "SeparateEntity", b2 =>
                                {
                                    b2.Property<Guid>("OwnedEntity_Owns_SeparateOneTestEntity_Owns_SeparateOne_SeparateOneId")
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("IntColumn")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("StringColumn")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("OwnedEntity_Owns_SeparateOneTestEntity_Owns_SeparateOne_SeparateOneId");

                                    b2.ToTable("SeparateEntitiesOne_SeparateOne_Inner", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("OwnedEntity_Owns_SeparateOneTestEntity_Owns_SeparateOne_SeparateOneId");
                                });

                            b1.Navigation("SeparateEntity")
                                .IsRequired();
                        });

                    b.Navigation("SeparateEntity")
                        .IsRequired();
                });

            modelBuilder.Entity("Thinktecture.TestDatabaseContext.TestEntity", b =>
                {
                    b.Navigation("Children");
                });
#pragma warning restore 612, 618
        }
    }
}