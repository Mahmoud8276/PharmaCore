using System;

namespace Pharmacy.System.Core.Interfaces.ISpecificationParams
{
    public interface IProductStockGroupSpecParams : IBaseSpecParams
    {
        public string? BatchNumber { get; set; }
        public int? RepositoryId { get; set; }
        public int? SupplierId { get; set; }
        public DateOnly? FromExpiryDate { get; set; }
        public DateOnly? ToExpiryDate { get; set; }
    }
}
