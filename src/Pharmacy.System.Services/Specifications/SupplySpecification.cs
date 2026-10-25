using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using System;
using System.Linq.Expressions;

namespace Pharmacy.System.Services.Specifications
{
    public class SupplySpecification : BaseSpecification<Supply, int>
    {
        public SupplySpecification(ISupplySpecParams specParams, bool isCount = false) : base(BuildCriteria(specParams))
        {
            if(!isCount)
            {
                ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
                AddOrderBy(x => x.Id);

                AddInclude(x => x.Supplier);
                AddInclude(x => x.Items);
                AddInclude(x => x.Payments);
                AddInclude(x => x.Installments);
                AddInclude("Installments.Payments");
                AddInclude(x => x.Returns);
                AddInclude(x => x.Repository);
            }
        }

        public SupplySpecification(int supplyId) : base(x => x.Id == supplyId)
        {
            AddInclude(x => x.Supplier);
            AddInclude(x => x.Items);
            AddInclude(x => x.Payments);
            AddInclude(x => x.Installments);
            AddInclude("Installments.Payments");
            AddInclude(x => x.Returns);
            AddInclude(x => x.Repository);
        }

        private static Expression<Func<Supply, bool>> BuildCriteria(ISupplySpecParams specParams)
        {
            var paymentStatus = specParams.PaymentStatus;
            var paymentPlan = specParams.PaymentPlan;
            var isConfirmed= specParams.IsConfirmed;
            var fromDate = specParams.FromDate;
            var toDate = specParams.ToDate;
            var supplierId = specParams.SupplierId;

            return x => (
            (paymentStatus == null || x.PaymentStatus == paymentStatus) &&
            (paymentPlan == null || x.PaymentPlan == paymentPlan) &&
            (isConfirmed == null || x.IsConfirmed == isConfirmed) &&
            (fromDate == null || x.ReceivedAt >= fromDate) &&
            (toDate == null || x.ReceivedAt <= toDate) && 
            (supplierId == null || x.SupplierId == supplierId));
        }
    }
}
