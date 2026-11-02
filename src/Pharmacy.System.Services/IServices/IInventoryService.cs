using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface IInventoryService
    {
        public Task GetAllAsync(InventorySpecParams spec);
    }
}
