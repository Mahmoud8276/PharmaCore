using Pharmacy.System.Core.Enums;
using System;

namespace Pharmacy.System.Core.Dtos.SupplyPaymentDtos
{
    public class SupplyPaymentDetailsDto
    {
        public int Id { get; set; }
        public int? SupplyInstallmentId { get; set; }
        public int SupplyId { get; set; }
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentMethod Method { get; set; }

    }
}
