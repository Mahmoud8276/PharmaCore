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
        public bool UserPaidConfirmation { get; set; } = false;
        public bool IsConfirmed { get; set; } = false;

        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public List<SupplyItem> Items { get; set; } = new List<SupplyItem>();
    }
}
