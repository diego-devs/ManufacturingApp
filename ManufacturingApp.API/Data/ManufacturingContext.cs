using ManufacturingApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ManufacturingApp.Data
{
    public class ManufacturingContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeProduct> RecipeProducts { get; set; }
        public DbSet<RecipeRawMaterial> RecipeRawMaterials { get; set;}
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierRawMaterial> SuppliersRawMaterials { get; set; }
        public ManufacturingContext(DbContextOptions<ManufacturingContext> options) : base(options) // Typed DbContextOptions
        {
        }
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
