using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.System.Core.Models;

namespace Pharmacy.System.Infrastructure.ModelsConfig
{
    public class SupplyItemConfig : IEntityTypeConfiguration<SupplyItem>
    {
        public void Configure(EntityTypeBuilder<SupplyItem> builder)
        {
            builder.Property(x => x.Quantity).HasPrecision(18, 4);
            builder.Property(x => x.PricePerUnit).HasPrecision(18, 4);
            builder.Property(x => x.BaseUnitsPerReceivedUnit).HasPrecision(18, 4);
            builder.Property(x => x.EffectiveUnitCost).HasPrecision(18, 4);
            
            builder.HasOne(x => x.Supply)
                   .WithMany(y => y.Items)
                   .HasForeignKey(x => x.SupplyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                   .WithMany()
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PackagingLevel)
                   .WithMany()
                   .HasForeignKey(x => x.PackagingLevelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.ProductSnapshot, b =>
            {
                b.ToJson();
            });

            builder.OwnsMany(x => x.PackagingLevelsSnapshot, b =>
            {
                b.ToJson();
                b.Property(p => p.QuantityOfChildPackage).HasPrecision(18, 4);
                b.Property(p => p.QuantityOfBaseUnit).HasPrecision(18, 4);
            });
        }
    }
}
