using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using System;

namespace Pharmacy.System.Services.SpecificationParams
{
    public class ProductStockGroupSpecParams : BaseSpecParams, IProductStockGroupSpecParams
    {
        public string? BatchNumber { get; set; }
        public int? RepositoryId { get; set; }
        public int? SupplierId { get; set; }
        public DateOnly? FromExpiryDate { get; set; }
        public DateOnly? ToExpiryDate { get; set; }
    }
}
