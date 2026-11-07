using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Services;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;
        private readonly IInventoryItemService _inventoryItemService;
        public InventoryController(
            IInventoryService inventoryService,
            IInventoryItemService inventoryItemService)
        {
            _inventoryService = inventoryService;
            _inventoryItemService = inventoryItemService;
        }


        [HttpGet]
        public async Task<IActionResult> GetInventoryProductsAsync([FromQuery] InventorySpecParams specParams)
        {
            return FromResponse(
                await _inventoryService.GetInventoryProductsAsync(specParams));
        }

        [HttpGet("products/{productId:int}")]
        public async Task<IActionResult> GetProductInventoryOverviewAsync(int productId)
        {
            return FromResponse(
                await _inventoryService.GetProductInventoryOverviewAsync(productId));
        }

        [HttpGet("products/{productId:int}/stock-groups")]
        public async Task<IActionResult> GetProductStockGroupsAsync(
            int productId,
            [FromQuery] ProductStockGroupSpecParams specParams)
        {
            return FromResponse(
                await _inventoryService.GetProductStockGroupsAsync(productId, specParams));
        }

        [HttpGet("inventory-items")]
        public async Task<IActionResult> GetAllInventoryItemsAsync([FromQuery] InventoryItemSpecParams specParams)
        {
            return FromResponse(
                await _inventoryItemService.GetAllAsync(specParams));
        }

        [HttpGet("inventory-items/{Id}")]
        public async Task<IActionResult> GetInventoryItemByIdAsync(int Id)
        {
            return FromResponse(
                await _inventoryItemService.GetByIdAsync(Id));
        }

    }
}
