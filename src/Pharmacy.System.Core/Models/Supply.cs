using Pharmacy.System.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.System.Core.Models
{
    public class Supply : BaseModel<int>
    {
        public decimal TotalCost { get; set; }
        public decimal DiscountPercintage { get; set; } = 0;
        public decimal FinalCost => (TotalCost - (TotalCost * DiscountPercintage));
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
