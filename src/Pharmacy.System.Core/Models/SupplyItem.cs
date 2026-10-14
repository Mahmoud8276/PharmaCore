using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.System.Core.Models
{
    public class SupplyItem : BaseModel<int>
    {
        [ForeignKey("Supply")]
        public int SupplyId { get; set; }
        public Supply Supply { get; set; } = null!;

        public decimal Quantity { get; set; }
        public decimal BaseUnitsPerReceivedUnit { get; set; }
        public decimal TotalBaseUnits { get; set; }
        public string UnitName { get; set; } = null!;
        public decimal PricePerUnit { get; set; }
        public decimal EffectiveUnitCost { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public ProductSnapshot ProductSnapshot { get; set; } = null!;
        public List<PackagingLevelSnapshot> PackagingLevelsSnapshot { get; set; } = new List<PackagingLevelSnapshot>();

        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("PackagingLevel")]
        public int? PackagingLevelId { get; set; }
        public ProductPackagingLevel? PackagingLevel { get; set; }
    }
}
