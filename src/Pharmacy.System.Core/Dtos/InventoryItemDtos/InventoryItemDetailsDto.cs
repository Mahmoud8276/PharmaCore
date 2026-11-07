using Pharmacy.System.Core.Dtos.ProductBatchDtos;
using Pharmacy.System.Core.Dtos.ProductDtos;
using Pharmacy.System.Core.Dtos.RepositoryDtos;

namespace Pharmacy.System.Core.Dtos.InventoryItemDtos
{
    public class InventoryItemDetailsDto
    {
        public int Id { get; set; }
        public ProductDetailsDto Product { get; set; }
        public RepositoryDetailsDto Repository { get; set; }
        public ProductBatchDetailsDto ProductBatch { get; set; }
        public string ReceivedQuantity { get; set; }
        public string RemainingQuantity { get; set; }
        public decimal RemainingQuantitySellingPrice { get; set; }
    }
}
