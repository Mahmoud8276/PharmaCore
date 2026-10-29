using Pharmacy.System.Core.Enums;
using System;

namespace Pharmacy.System.Core.Interfaces.ISpecificationParams
{
    public interface ISupplyInstallmentSpecParams : IBaseSpecParams
    {
        public int? SupplyId { get; set; }
        public string? Notes { get; set; }
        public decimal? FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public DateOnly? FromDueDate { get; set; }
        public DateOnly? ToDueDate { get; set; }
        public SupplyInstallmentStatus? Status { get; set; }
    }
}
