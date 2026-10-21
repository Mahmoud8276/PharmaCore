using Pharmacy.System.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.System.Core.Models
{
    public class Supply : BaseModel<int>
    {
        public decimal TotalCost { get; set; }
        public decimal DiscountPercentage { get; set; } = 0;
        public decimal FinalCost { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateOnly ReceivedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public SupplyPaymentPlan PaymentPlan { get; set; }
        public bool IsConfirmed { get; set; } = false;

        public bool IsPaid => (PaymentStatus == PaymentStatus.FullyPaid);

        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        [ForeignKey("Repository")]
        public int RepositoryId { get; set; }
        public Repository Repository { get; set; } = null!;

        public List<SupplyItem> Items { get; set; } = new List<SupplyItem>();
        public List<SupplyInstallment> Installments { get; set; } = new List<SupplyInstallment>();
        public List<SupplyPayment> Payments { get; set; } = new List<SupplyPayment>();
    }
}
