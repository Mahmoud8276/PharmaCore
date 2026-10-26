using Pharmacy.System.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Pharmacy.System.Core.Models
{
    public class Supply : BaseModel<int>
    {
        public decimal TotalCost { get; set; }
        public decimal DiscountPercentage { get; set; } = 0;
        public decimal FinalCost { get; set; }
        public decimal RemainingAmount => ComputeRemainingAmount();
        public decimal PaidAmount => ComputePaidAmount();
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
        public List<SupplyReturn> Returns { get; set; } = new List<SupplyReturn>();


        private decimal ComputeRemainingAmount()
        {
            decimal totalPaid = ComputePaidAmount();
            decimal totalReturned = Returns.Sum(x => x.TotalAmount);
            
            return Math.Round(Math.Max(FinalCost - totalPaid - totalReturned, 0));
        }

        private decimal ComputePaidAmount()
        {
            return Payments.Sum(payment => payment.Amount);
        }

        public void RecalculatePaymentStatus()
        {
            var remainingAmount = RemainingAmount;

            PaymentStatus = remainingAmount == 0 ? PaymentStatus.FullyPaid :
                remainingAmount == FinalCost ? PaymentStatus.NotPaid :
                PaymentStatus.PartiallyPaid;
        }
    }
}
