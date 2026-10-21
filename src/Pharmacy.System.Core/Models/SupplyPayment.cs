using Pharmacy.System.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.System.Core.Models
{
    public class SupplyPayment : BaseModel<int>
    {
        [ForeignKey("SupplyInstallment")]
        public int? SupplyInstallmentId { get; set; }
        public SupplyInstallment? SupplyInstallment { get; set; }


        [ForeignKey("Supply")]
        public int SupplyId { get; set; }
        public Supply Supply { get; set; }


        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateOnly PaidAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentMethod Method { get; set; } = PaymentMethod.Other;
    }
}
