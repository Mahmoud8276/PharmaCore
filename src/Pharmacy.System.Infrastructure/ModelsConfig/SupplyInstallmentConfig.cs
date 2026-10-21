using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharmacy.System.Core.Models;

namespace Pharmacy.System.Infrastructure.ModelsConfig
{
    public class SupplyInstallmentConfig : IEntityTypeConfiguration<SupplyInstallment>
    {
        public void Configure(EntityTypeBuilder<SupplyInstallment> builder)
        {
            builder.Property(x => x.Amount)
                .HasPrecision(18, 4);
        }
    }
}
