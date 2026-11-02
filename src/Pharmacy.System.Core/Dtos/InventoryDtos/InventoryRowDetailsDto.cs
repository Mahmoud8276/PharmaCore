using Pharmacy.System.Core.Dtos.ProductDtos;
using Pharmacy.System.Core.Enums;

namespace Pharmacy.System.Core.Dtos.InventoryDtos
{
    public class InventoryRowDetailsDto
    {
        public ProductDetailsDto Product {  get; set; }
        public string Quantity { get; set; }
        public InventoryStatus Status { get; set; }
    }
}
