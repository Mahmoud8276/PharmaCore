using Pharmacy.System.Core.Interfaces.ISpecificationParams;

namespace Pharmacy.System.Services.SpecificationParams
{
    public class InventoryItemSpecParams : BaseSpecParams, IInventoryItemSpecParams
    {
        public int? RepositoryId { get; set; }
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Barcode { get; set; }
    }
}
