using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using System.Linq.Expressions;
using System;

namespace Pharmacy.System.Services.Specifications
{
    public class SupplyPaymentSpecification : BaseSpecification<SupplyPayment, int>
    {
        public SupplyPaymentSpecification(
            ISupplyPaymentSpecParams specParams, 
            bool isCount = false) : base(BuildCriteria(specParams))
        {
            if(!isCount)
            {
                ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
                AddOrderBy(x => x.Id);
            }
        }


        private static Expression<Func<SupplyPayment, bool>> BuildCriteria(ISupplyPaymentSpecParams specParams)
        {
            var supplyId = specParams.SupplyId;
            var installmentId = specParams.InstallmentId;
            var note = specParams.Note;
            var paymentMethod = specParams.PaymentMethod;
            var fromAmount = specParams.FromAmount;
            var toAmount = specParams.ToAmount;
            var fromDate = specParams.FromDate;
            var toDate = specParams.ToDate;

            return x => (
            (supplyId == null || x.SupplyId == supplyId) &&
            (installmentId == null || x.SupplyInstallmentId == installmentId) &&
            (note == null || x.Notes.Contains(note.ToLower())) &&
            (paymentMethod == null || x.Method == paymentMethod) &&
            (fromAmount == null || x.Amount >= fromAmount) &&
            (toAmount == null || x.Amount <= toAmount) &&
            (fromDate == null || x.PaidAt <= fromDate) &&
            (toDate == null || x.PaidAt <= toDate));
        }
    }
}
