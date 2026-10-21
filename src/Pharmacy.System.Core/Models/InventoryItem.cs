using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.System.Core.Models
{
    public class InventoryItem : BaseModel<int>
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;


        [ForeignKey("Repository")]
        public int RepositoryId { get; set; }
        public Repository Repository { get; set; } = null!;


        [ForeignKey("SupplyItem")]
        public int SupplyItemId { get; set; }
        public SupplyItem SupplyItem { get; set; } = null!;

        public DateOnly ExpiryDate { get; set; }

        public decimal ReceivedBaseUnits { get; set; } 
        public decimal OnHandBaseUnits { get; set; }
        public decimal PricePerBaseUnits { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
