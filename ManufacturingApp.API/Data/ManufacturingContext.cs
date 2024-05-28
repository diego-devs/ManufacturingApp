using ManufacturingApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ManufacturingApp.Data
{
    /// <summary>
    /// Represents the database context for the manufacturing application.
    /// </summary>
    public class ManufacturingContext : DbContext
    {
        /// <summary>
        /// Gets or sets the DbSet for products.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for raw materials.
        /// </summary>
        public DbSet<RawMaterial> RawMaterials { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for recipes.
        /// </summary>
        public DbSet<Recipe> Recipes { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for recipe products.
        /// </summary>
        public DbSet<RecipeProduct> RecipeProducts { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for recipe raw materials.
        /// </summary>
        public DbSet<RecipeRawMaterial> RecipeRawMaterials { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for suppliers.
        /// </summary>
        public DbSet<Supplier> Suppliers { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for supplier raw materials.
        /// </summary>
        public DbSet<SupplierRawMaterial> SuppliersRawMaterials { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ManufacturingContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ManufacturingContext(DbContextOptions<ManufacturingContext> options) : base(options) // Typed DbContextOptions
        {
        }
        /// <summary>
        /// Configures the model for the context.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SupplierRawMaterial many-to-many configuration
            modelBuilder.Entity<SupplierRawMaterial>()
                .HasKey(srm => new { srm.SupplierId, srm.RawMaterialId });

            modelBuilder.Entity<SupplierRawMaterial>()
                .HasOne(srm => srm.Supplier)
                .WithMany(s => s.SupplierRawMaterials)
                .HasForeignKey(srm => srm.SupplierId);

            modelBuilder.Entity<SupplierRawMaterial>()
                .HasOne(srm => srm.RawMaterial)
                .WithMany(rm => rm.SupplierRawMaterials)
                .HasForeignKey(srm => srm.RawMaterialId);

            // RecipeRawMaterial many-to-many configuration
            modelBuilder.Entity<RecipeRawMaterial>()
                .HasKey(rrm => new { rrm.RecipeId, rrm.RawMaterialId });

            modelBuilder.Entity<RecipeRawMaterial>()
                .HasOne(rrm => rrm.Recipe)
                .WithMany(r => r.RecipeRawMaterials)
                .HasForeignKey(rrm => rrm.RecipeId);

            modelBuilder.Entity<RecipeRawMaterial>()
                .HasOne(rrm => rrm.RawMaterial)
                .WithMany(rm => rm.RecipeRawMaterials)
                .HasForeignKey(rrm => rrm.RawMaterialId);

            // RecipeProduct many-to-many configuration
            modelBuilder.Entity<RecipeProduct>()
                .HasKey(rp => new { rp.RecipeId, rp.ProductId });

            modelBuilder.Entity<RecipeProduct>()
                .HasOne(rp => rp.Recipe)
                .WithMany(r => r.RecipeProducts)
                .HasForeignKey(rp => rp.RecipeId);

            modelBuilder.Entity<RecipeProduct>()
                .HasOne(rp => rp.Product)
                .WithMany(p => p.RecipeProducts)
                .HasForeignKey(rp => rp.ProductId);

        }


    }
}
