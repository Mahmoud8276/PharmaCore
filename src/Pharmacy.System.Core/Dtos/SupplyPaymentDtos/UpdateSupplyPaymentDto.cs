using Pharmacy.System.Core.Attributes;
using Pharmacy.System.Core.Enums;
using System;

namespace Pharmacy.System.Core.Dtos.SupplyPaymentDtos
{
    public class UpdateSupplyPaymentDto
    {
        public string? Notes { get; set; }

        [NotInFuture(ErrorMessage = "Payment date can not be in the future!")]
        public DateOnly? PaidAt { get; set; }
        public PaymentMethod? Method { get; set; }

    }
}
