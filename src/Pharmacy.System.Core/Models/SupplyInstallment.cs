using Pharmacy.System.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Pharmacy.System.Core.Models
{
    public class SupplyInstallment : BaseModel<int>
    {
        [ForeignKey("Supply")]
        public int SupplyId { get; set; }
        public Supply Supply { get; set; }

        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public decimal RemainingAmount => ComputeRemainingAmount();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateOnly DueDate { get; set; }
        public SupplyInstallmentStatus Status { get; set; } = SupplyInstallmentStatus.Pending;

        public List<SupplyPayment> Payments { get; set; } = new List<SupplyPayment>();
    
        
        private decimal ComputeRemainingAmount()
        {
            var paidAmount = Payments.Sum(x => x.Amount);

            return Math.Round(Math.Max(Amount - paidAmount, 0));
        }
    }
}
