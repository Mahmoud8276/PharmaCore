using Pharmacy.System.Core.Dtos.InventoryItemDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using Pharmacy.System.Services.Specifications;

namespace Pharmacy.System.Services.Services
{
    public class InventoryItemService
        : ReadService<InventoryItem, int, InventoryItemSpecParams, InventoryItemDetailsDto>, IInventoryItemService
    {
        public InventoryItemService(IGenericRepository<InventoryItem, int> repository) 
            : base(repository)
        {
        }

        protected override ISpecification<InventoryItem, int> BuildSpec(InventoryItemSpecParams specParams, bool isCountQuery)
        {
            return new InventoryItemSpcification(specParams, isCountQuery);
        }
    }
}
