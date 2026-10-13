
namespace Pharmacy.System.Core.Interfaces.ISpecificationParams
{
    public interface ISupplierSpecParams : IBaseSpecParams
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
