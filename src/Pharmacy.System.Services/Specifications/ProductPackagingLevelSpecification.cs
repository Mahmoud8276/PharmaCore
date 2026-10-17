using Pharmacy.System.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pharmacy.System.Services.Specifications
{
    public class ProductPackagingLevelSpecification : BaseSpecification<ProductPackagingLevel, int>
    {
        public ProductPackagingLevelSpecification(int levelId) : base(x=>x.Id == levelId) 
        {
            AddInclude(x => x.Product);
            AddInclude(x => x.PackagingUnit);
        }

        public ProductPackagingLevelSpecification(int? productId, int? packagingUnitId)
            : base(x=> 
            (!productId.HasValue || x.ProductId == productId) &&
            (!packagingUnitId.HasValue || x.PackagingUnitId == packagingUnitId))
        {
            AddInclude(x => x.Product);
            AddInclude(x => x.PackagingUnit);
        }

        public ProductPackagingLevelSpecification(IEnumerable<int> packagingLevelIds):
            base(x => packagingLevelIds.ToHashSet().Contains(x.Id))
        {
            AddInclude(x => x.Product);
            AddInclude(x => x.PackagingUnit);
        }
    }
}
