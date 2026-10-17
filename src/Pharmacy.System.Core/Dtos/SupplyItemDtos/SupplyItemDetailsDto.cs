using System;

namespace Pharmacy.System.Core.Dtos.SupplyItemDtos
{
    public class SupplyItemDetailsDto
    {
        public int Id { get; set; }
        public string UnitName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public decimal EffectiveUnitCost { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductCategoryName { get; set; } = null!;
        public string ProductFormName { get; set; } = null!;

    }
}
