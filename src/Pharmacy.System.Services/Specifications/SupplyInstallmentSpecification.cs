using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using System.Linq.Expressions;
using System;

namespace Pharmacy.System.Services.Specifications
{
    public class SupplyInstallmentSpecification : BaseSpecification<SupplyInstallment, int>
    {
        public SupplyInstallmentSpecification(
            ISupplyInstallmentSpecParams specParams, 
            bool isCount = false) : base(BuildCriteria(specParams))
        {
            if (!isCount)
            {
                ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
                AddOrderBy(x => x.Id);

                AddInclude(x => x.Payments);
            }
        }

        public SupplyInstallmentSpecification(int installmentId) : base(x=>x.Id ==  installmentId)
        {
            AddInclude(x => x.Payments);
        }


        private static Expression<Func<SupplyInstallment, bool>> BuildCriteria(ISupplyInstallmentSpecParams specParams)
        {
            var supplyId = specParams.SupplyId;
            var notes = specParams.Notes;
            var fromAmount = specParams.FromAmount;
            var toAmount = specParams.ToAmount;
            var fromDueDate = specParams.FromDueDate;
            var toDueDate = specParams.ToDueDate;
            var status = specParams.Status;

            return x => (
            (supplyId == null || x.SupplyId == supplyId) &&
            (notes == null || x.Notes.Contains(notes.ToLower())) &&
            (fromDueDate == null || x.DueDate >= fromDueDate) &&
            (toDueDate == null || x.DueDate <= toDueDate) &&
            (fromAmount == null || x.Amount >= fromAmount) &&
            (toAmount == null || x.Amount == toAmount) &&
            (status == null || x.Status == status));
        }
    }
}
