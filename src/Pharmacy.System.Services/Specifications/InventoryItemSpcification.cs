using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Pharmacy.System.Services.Specifications
{
    public class InventoryItemSpcification : BaseSpecification<InventoryItem, int>
    {
        public InventoryItemSpcification(IInventoryItemSpecParams specParams, bool isCount = false)
            : base(BuildCriteria(specParams))
        {
            if (!isCount)
            {
                AddInclude(x => x.Product);
                AddInclude(x => x.Product.Category);
                AddInclude(x => x.Product.Barcodes);
                AddInclude(x => x.Repository);
                AddInclude(x => x.ProductBatch);

                ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

                AddOrderBy(x => x.Product.Name);
            }
        }

        public InventoryItemSpcification(int InventoryItemId)
            : base(x=>x.Id == InventoryItemId)
        {
            AddInclude(x => x.Product);
            AddInclude(x => x.Product.Category);
            AddInclude(x => x.Product.Barcodes);
            AddInclude(x => x.Repository);
            AddInclude(x => x.ProductBatch);
        }

        private static Expression<Func<InventoryItem, bool>> BuildCriteria(IInventoryItemSpecParams specParams)
        {
            return x =>
                (!specParams.RepositoryId.HasValue || x.RepositoryId == specParams.RepositoryId) &&
                (!specParams.CategoryId.HasValue || x.Product.CategoryId == specParams.CategoryId) &&
                (string.IsNullOrEmpty(specParams.Name) || x.Product.Name.Contains(specParams.Name.ToLower())) &&
                (string.IsNullOrEmpty(specParams.Barcode) || x.Product.Barcodes.Any(x=>x.Barcode == specParams.Barcode));
        }
    }
}
