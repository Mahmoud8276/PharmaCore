using Pharmacy.System.Core.Dtos.ProductDtos;
using Pharmacy.System.Core.Dtos.RepositoryDtos;
using Pharmacy.System.Core.Enums;
using System.Collections.Generic;

namespace Pharmacy.System.Core.Dtos.InventoryDtos
{
    public class ProductStockDetailsDto
    {
        public ProductDetailsDto Product { get; set; }
        public string TotalQuantity { get; set; }
        public InventoryStatus Status { get; set; } = InventoryStatus.OutOfStock;
        public List<TotalQuantityPerRepositoryDetailsDto> QuantityPerRepository { get; set; } = new();
        public decimal TotalPurchaseCost { get; set; } = default;
        public decimal TotalSellingPrice { get; set; } = default;
    }

    public class TotalQuantityPerRepositoryDetailsDto
    {
        public RepositoryDetailsDto Repository { get; set; }
        public string Quantity { get; set; }
    }
}
