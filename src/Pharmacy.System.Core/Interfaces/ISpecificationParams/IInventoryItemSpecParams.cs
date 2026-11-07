namespace Pharmacy.System.Core.Interfaces.ISpecificationParams
{
    public interface IInventoryItemSpecParams : IBaseSpecParams
    {
        public int? RepositoryId { get; set; }
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Barcode { get; set; }
    }
}
