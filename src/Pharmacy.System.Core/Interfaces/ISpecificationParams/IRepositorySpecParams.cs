namespace Pharmacy.System.Core.Interfaces.ISpecificationParams
{
    public interface IRepositorySpecParams : IBaseSpecParams
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
