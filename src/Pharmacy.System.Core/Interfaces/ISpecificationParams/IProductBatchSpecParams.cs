using System;

namespace Pharmacy.System.Core.Interfaces.ISpecificationParams
{
    public interface IProductBatchSpecParams : IBaseSpecParams
    {
        public string? Number { get; set; }
        public DateOnly? FromExpiryDate { get; set; }
        public DateOnly? ToExpiryDate { get; set; }
        public DateOnly? FromManufactureDate { get; set; }
        public DateOnly? ToManufactureDate { get; set; }
    }
}
