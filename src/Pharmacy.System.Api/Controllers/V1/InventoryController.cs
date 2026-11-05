using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] InventorySpecParams specParams)
        {
            return FromResponse(
                await _inventoryService.GetAllAsync(specParams));
        }

        [HttpGet("products/{productId:int}")]
        public async Task<IActionResult> GetByProductIdAsync(int productId)
        {
            return FromResponse(
                await _inventoryService.GetByProductIdAsync(productId));
        }

        [HttpGet("products/{productId:int}/stock-groups")]
        public async Task<IActionResult> GetAllProductStockGroupsAsync(
            int productId,
            [FromQuery] ProductStockGroupSpecParams specParams)
        {
            return FromResponse(
                await _inventoryService.GetAllProductStockGroupsAsync(productId, specParams));
        }
    }
}
