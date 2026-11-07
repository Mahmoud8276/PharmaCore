using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.SpecificationParams;

namespace Pharmacy.System.Services.IServices
{
    public interface IInventoryItemService 
        : IReadService<InventoryItem, int, InventoryItemSpecParams>
    {
    }
}
