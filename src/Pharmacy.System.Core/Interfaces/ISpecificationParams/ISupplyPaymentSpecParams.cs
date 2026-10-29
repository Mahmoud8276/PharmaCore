using Pharmacy.System.Core.Enums;
using System;

namespace Pharmacy.System.Core.Interfaces.ISpecificationParams
{
    public interface ISupplyPaymentSpecParams : IBaseSpecParams
    {
        public int? SupplyId { set; get; }
        public int? InstallmentId { set; get; }
        public string? Note { set; get; }
        public PaymentMethod? PaymentMethod { set; get; }
        public decimal? FromAmount { set; get; }
        public decimal? ToAmount { set; get; }
        public DateOnly? FromDate { set; get; }
        public DateOnly? ToDate { set; get; }
    }
}
