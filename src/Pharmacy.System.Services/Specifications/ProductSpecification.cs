using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pharmacy.System.Services.Specifications
{
    public class ProductSpecification : BaseSpecification<Product, int>
    {

        public ProductSpecification(IProductSpecParams specParams, bool isCount = false) : base(BuildCriteria(specParams))
        {
            if (!isCount)
            {
                ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
                AddOrderBy(x => x.Name);
            }

            AddInclude(x => x.Category);
            AddInclude(x => x.Barcodes);
            AddInclude(x => x.ProductForm);
            AddInclude(x => x.Manufacturer);
            AddInclude(x => x.ProductActiveIngredients);
            AddInclude(x => x.ProductPackagingLevels);
            AddInclude(x => x.DefaultPackaging);
            AddInclude("DefaultPackaging.PackagingUnit");
            AddInclude("ProductActiveIngredients.ActiveIngredient");
        }

        public ProductSpecification(int ProductId) : base(x => x.Id == ProductId)
        {
            AddInclude(x => x.Category);
            AddInclude(x => x.Barcodes);
            AddInclude(x => x.ProductForm);
            AddInclude(x => x.Manufacturer);
            AddInclude(x => x.ProductActiveIngredients);
            AddInclude(x => x.ProductPackagingLevels);
            AddInclude(x => x.DefaultPackaging);
            AddInclude("DefaultPackaging.PackagingUnit");
            AddInclude("ProductActiveIngredients.ActiveIngredient");
        }

        public ProductSpecification(IEnumerable<int> productIds)
            : base(x => productIds.ToHashSet().Contains(x.Id))
        {
            AddInclude(x => x.Category);
            AddInclude(x => x.Barcodes);
            AddInclude(x => x.ProductForm);
            AddInclude(x => x.Manufacturer);
            AddInclude(x => x.ProductActiveIngredients);
            AddInclude(x => x.ProductPackagingLevels);
            AddInclude(x => x.DefaultPackaging);
            AddInclude("DefaultPackaging.PackagingUnit");
            AddInclude("ProductActiveIngredients.ActiveIngredient");
        }

        private static Expression<Func<Product, bool>> BuildCriteria(IProductSpecParams specParams)
        {
            var name = specParams.Name?.ToLower();
            var description = specParams.Description?.ToLower();

            return x =>
                (string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name)) &&
                (string.IsNullOrEmpty(description) || x.Description.ToLower().Contains(description)) &&
                (specParams.ManufacturerId == null || x.ManufacturerId == specParams.ManufacturerId) &&
                (specParams.CategoryId == null || x.CategoryId == specParams.CategoryId);
        }
    }
}
