using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.System.Core.Models;

namespace Pharmacy.System.Infrastructure.ModelsConfig
{
    public class SupplyConfig : IEntityTypeConfiguration<Supply>
    {
        public void Configure(EntityTypeBuilder<Supply> builder)
        {
            builder.HasOne(s => s.Supplier)
                   .WithMany()
                   .HasForeignKey(s => s.SupplierId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TotalCost).HasPrecision(18, 4);
            builder.Property(x => x.DiscountPercentage).HasPrecision(18, 4);
        }
    }
}
