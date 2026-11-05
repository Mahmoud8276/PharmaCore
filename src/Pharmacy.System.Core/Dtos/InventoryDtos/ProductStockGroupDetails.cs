using System;
using System.Collections.Generic;

namespace Pharmacy.System.Core.Dtos.InventoryDtos
{
    public class ProductStockGroupDetails
    {
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public List<string> ProductBarcodes { get; set; } = new();
        public string RepositoryName { get; set; }
        public string ReminingDaysToExpiry { get; set; }
        public decimal TotalPurchaseCost { get; set; }
        public decimal RemainingQuantitySellingPrice { get; set; }
        public string ReceivedQuantity { get; set; }
        public string RemainingQuantity { get; set; }
        public string SupplierName { get; set; }
        public DateOnly ExpiryDate { get; set; }
    }
}
