﻿// <auto-generated />
using ManufacturingApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ManufacturingApp.API.Migrations
{
    [DbContext(typeof(ManufacturingContext))]
    [Migration("20240528000014_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ManufacturingApp.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "name");

                    b.Property<decimal>("SellingPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasAnnotation("Relational:JsonPropertyName", "sellingPrice");

                    b.HasKey("Id");

                    b.ToTable("Product");

                    b.HasAnnotation("Relational:JsonPropertyName", "product");
                });

            modelBuilder.Entity("ManufacturingApp.Models.RawMaterial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "name");

                    b.HasKey("Id");

                    b.ToTable("RawMaterial");

                    b.HasAnnotation("Relational:JsonPropertyName", "rawMaterial");
                });

            modelBuilder.Entity("ManufacturingApp.Models.Recipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "name");

                    b.HasKey("Id");

                    b.ToTable("Recipe");

                    b.HasAnnotation("Relational:JsonPropertyName", "recipe");
                });

            modelBuilder.Entity("ManufacturingApp.Models.RecipeProduct", b =>
                {
                    b.Property<int>("RecipeId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "recipeId");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "productId");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(18,2)")
                        .HasAnnotation("Relational:JsonPropertyName", "quantity");

                    b.HasKey("RecipeId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("RecipeProduct");

                    b.HasAnnotation("Relational:JsonPropertyName", "recipeProducts");
                });

            modelBuilder.Entity("ManufacturingApp.Models.RecipeRawMaterial", b =>
                {
                    b.Property<int>("RecipeId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "recipeId");

                    b.Property<int>("RawMaterialId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "rawMaterialId");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(18,2)")
                        .HasAnnotation("Relational:JsonPropertyName", "quantity");

                    b.HasKey("RecipeId", "RawMaterialId");

                    b.HasIndex("RawMaterialId");

                    b.ToTable("RecipeRawMaterial");

                    b.HasAnnotation("Relational:JsonPropertyName", "recipeRawMaterials");
                });

            modelBuilder.Entity("ManufacturingApp.Models.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "name");

                    b.HasKey("Id");

                    b.ToTable("Supplier");

                    b.HasAnnotation("Relational:JsonPropertyName", "supplier");
                });

            modelBuilder.Entity("ManufacturingApp.Models.SupplierRawMaterial", b =>
                {
                    b.Property<int>("SupplierId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "supplierId");

                    b.Property<int>("RawMaterialId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "rawMaterialId");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)")
                        .HasAnnotation("Relational:JsonPropertyName", "price");

                    b.HasKey("SupplierId", "RawMaterialId");

                    b.HasIndex("RawMaterialId");

                    b.ToTable("SupplierRawMaterial");

                    b.HasAnnotation("Relational:JsonPropertyName", "supplierRawMaterials");
                });

            modelBuilder.Entity("ManufacturingApp.Models.RecipeProduct", b =>
                {
                    b.HasOne("ManufacturingApp.Models.Product", "Product")
                        .WithMany("RecipeProducts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ManufacturingApp.Models.Recipe", "Recipe")
                        .WithMany("RecipeProducts")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("ManufacturingApp.Models.RecipeRawMaterial", b =>
                {
                    b.HasOne("ManufacturingApp.Models.RawMaterial", "RawMaterial")
                        .WithMany("RecipeRawMaterials")
                        .HasForeignKey("RawMaterialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ManufacturingApp.Models.Recipe", "Recipe")
                        .WithMany("RecipeRawMaterials")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RawMaterial");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("ManufacturingApp.Models.SupplierRawMaterial", b =>
                {
                    b.HasOne("ManufacturingApp.Models.RawMaterial", "RawMaterial")
                        .WithMany("SupplierRawMaterials")
                        .HasForeignKey("RawMaterialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ManufacturingApp.Models.Supplier", "Supplier")
                        .WithMany("SupplierRawMaterials")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RawMaterial");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("ManufacturingApp.Models.Product", b =>
                {
                    b.Navigation("RecipeProducts");
                });

            modelBuilder.Entity("ManufacturingApp.Models.RawMaterial", b =>
                {
                    b.Navigation("RecipeRawMaterials");

                    b.Navigation("SupplierRawMaterials");
                });

            modelBuilder.Entity("ManufacturingApp.Models.Recipe", b =>
                {
                    b.Navigation("RecipeProducts");

                    b.Navigation("RecipeRawMaterials");
                });

            modelBuilder.Entity("ManufacturingApp.Models.Supplier", b =>
                {
                    b.Navigation("SupplierRawMaterials");
                });
#pragma warning restore 612, 618
        }
    }
}
