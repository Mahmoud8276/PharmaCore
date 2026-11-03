using Pharmacy.System.Core.Enums;

namespace Pharmacy.System.Core.Dtos.SupplyPaymentDtos
{
    public class SupplyPaymentConfirmationDto
    {
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Other;
        public string? Notes { get; set; }
    }
}
