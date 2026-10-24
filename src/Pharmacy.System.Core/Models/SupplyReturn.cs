using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.System.Core.Models
{
    public class SupplyReturn : BaseModel<int>
    {
        [ForeignKey("Supply")]
        public int SupplyId { get; set; }
        public Supply Supply { get; set; } = null!;

        public decimal TotalAmount { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }
    }
}
