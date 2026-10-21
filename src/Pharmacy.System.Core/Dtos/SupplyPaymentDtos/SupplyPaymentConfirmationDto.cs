using Pharmacy.System.Core.Enums;

namespace Pharmacy.System.Core.Dtos.SupplyPaymentDtos
{
    public class SupplyPaymentConfirmationDto
    {
        public PaymentMethod PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }
}
