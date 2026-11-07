using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface IInventoryItemService
    {
        public Task<Response> GetAllAsync(InventoryItemSpecParams specParams);
        public Task<Response> GetByIdAsync(int id);
    }
}
