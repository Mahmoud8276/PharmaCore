using Pharmacy.System.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Pharmacy.System.Infrastructure.DbContexts
{
    public class AppDbContext : IdentityDbContext<AppUser, UserRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductForm> ProductForms { get; set; }
        public DbSet<ProductActiveIngredient> ProductActiveIngredients { get; set; }
        public DbSet<ActiveIngredient> ActiveIngredients { get; set; }
        public DbSet<UnitFamily> UnitFamilies { get; set; }
        public DbSet<BaseUnit> BaseUnits { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<PackagingUnit> PackagingUnits { get; set; }
        public DbSet<ProductPackagingLevel> ProductPackagingLevels { get; set; }
        public DbSet<Supply> Supplies { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
