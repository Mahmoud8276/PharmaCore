using Pharmacy.System.Core.Enums;
using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using System;

namespace Pharmacy.System.Services.SpecificationParams
{
    public class SupplyInstallmentSpecParams : BaseSpecParams, ISupplyInstallmentSpecParams
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
