using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using System;
using System.Linq.Expressions;

namespace Pharmacy.System.Services.Specifications
{
    public class ProductBatchSpecification : BaseSpecification<ProductBatch, int>
    {
        public ProductBatchSpecification(IProductBatchSpecParams specParams, bool isCount = false) 
            : base(BuildCriteria(specParams))
        {
            if (!isCount)
            {
                ApplyPagination((specParams.PageIndex-1) * specParams.PageSize, specParams.PageSize);
                AddOrderBy(x => x.Id);
            }
        }

        private static Expression<Func<ProductBatch, bool>> BuildCriteria(IProductBatchSpecParams specParams)
        {
            var number = specParams.Number?.Trim();
            var fromExpiryDate = specParams.FromExpiryDate;
            var toExpiryDate = specParams.ToExpiryDate;
            var fromManufactureDate = specParams.FromManufactureDate;
            var toManufactureDate = specParams.ToManufactureDate;

            return x =>
            ((string.IsNullOrEmpty(number) || x.Number.Contains(number.ToLower())) &&
            (fromExpiryDate == null || x.ExpiryDate >= fromExpiryDate) &&
            (toExpiryDate == null || x.ExpiryDate <= toExpiryDate) &&
            (fromManufactureDate == null || x.ManufactureDate >= fromManufactureDate) &&
            (toManufactureDate == null || x.ManufactureDate <= toManufactureDate));
        }
    }
}
