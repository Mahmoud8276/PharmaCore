using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using System;

namespace Pharmacy.System.Services.SpecificationParams
{
    public class ProductBatchSpecParams : BaseSpecParams, IProductBatchSpecParams
    {
        public string? Number { get; set; }
        public DateOnly? FromExpiryDate { get; set; }
        public DateOnly? ToExpiryDate { get; set; }
        public DateOnly? FromManufactureDate { get; set; }
        public DateOnly? ToManufactureDate { get; set; }
    }
}
