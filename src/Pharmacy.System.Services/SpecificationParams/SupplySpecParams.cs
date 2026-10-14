using Pharmacy.System.Core.Enums;
using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using System;

namespace Pharmacy.System.Services.SpecificationParams
{
    public class SupplySpecParams : BaseSpecParams, ISupplySpecParams
    {
        public PaymentStatus? PaymentStatus { get; set; }
        public bool? IsConfirmed { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public int? SupplierId { get; set; }
    }
}
