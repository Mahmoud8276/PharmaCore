using Pharmacy.System.Core.Enums;
using System;

namespace Pharmacy.System.Core.Interfaces.ISpecificationParams
{
    public interface ISupplySpecParams : IBaseSpecParams
    {
        public PaymentStatus? PaymentStatus { get; set; }
        public SupplyPaymentPlan? PaymentPlan { get; set; }
        public bool? IsConfirmed { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public int? SupplierId { get; set; }
    }
}
