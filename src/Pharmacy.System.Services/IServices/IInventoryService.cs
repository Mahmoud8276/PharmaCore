using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface IInventoryService
    {
        public Task<Response> GetAllAsync(InventorySpecParams specParams);
        public Task<Response> GetByProductIdAsync(int productId);
        public Task<Response> GetAllProductStockGroupsAsync(int productId, ProductStockGroupSpecParams specParams);
    }
}
