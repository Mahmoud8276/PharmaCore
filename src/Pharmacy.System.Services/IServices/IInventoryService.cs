using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface IInventoryService
    {
        public Task<Response> GetInventoryProductsAsync(InventorySpecParams specParams);
        public Task<Response> GetProductInventoryOverviewAsync(int productId);
        public Task<Response> GetProductStockGroupsAsync(int productId, ProductStockGroupSpecParams specParams);
    }
}
