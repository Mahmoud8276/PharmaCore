using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.System.Core.Models;

namespace Pharmacy.System.Infrastructure.ModelsConfig
{
    public class ProductBarcodeConfig : IEntityTypeConfiguration<ProductBarcode>
    {
        public void Configure(EntityTypeBuilder<ProductBarcode> builder)
        {
            builder.HasOne(x => x.Product)
                   .WithMany(x => x.Barcodes)
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x=> x.Barcode)
                   .IsUnique();
        }
    }
}
