using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.System.Core.Models;

namespace Pharmacy.System.Infrastructure.ModelsConfig
{
    public class SupplierCreditConfig : IEntityTypeConfiguration<SupplierCredit>
    {
        public void Configure(EntityTypeBuilder<SupplierCredit> builder)
        {
            builder.HasOne(x=>x.Supplier)
                   .WithMany(y=>y.Credits)
                   .HasForeignKey(x=>x.SupplierId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SupplyReturn)
                   .WithMany()
                   .HasForeignKey(x => x.SupplyReturnId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
