using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.System.Core.Models;

namespace Pharmacy.System.Infrastructure.ModelsConfig
{
    public class SupplierConfig : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.OwnsMany(s => s.PhoneNumbers, pn =>
            {
                pn.WithOwner().HasForeignKey("SupplierId");
                pn.Property<int>("Id");
                pn.HasKey("Id");
                pn.Property(p => p.Number).IsRequired();
                pn.HasIndex(p => p.Number).IsUnique();
                
                pn.HasOne<Supply>()
                .WithMany()
                .HasForeignKey("SupplierId")
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
