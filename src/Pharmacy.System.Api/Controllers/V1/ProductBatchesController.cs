using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.ProductBatchDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class ProductBatchesController : BaseController
    {
        private readonly IProductBatchService _productBatchService;
        public ProductBatchesController(IProductBatchService productBatchService)
        {
            _productBatchService = productBatchService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProductBatchDto dto)
        {
            return FromResponse(
                await _productBatchService.CreateAsync(dto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductBatchSpecParams specParams)
        {
            return FromResponse(
                await _productBatchService.GetAllAsync(specParams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return FromResponse(
                await _productBatchService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProductBatchDto dto)
        {
            return FromResponse(
                await _productBatchService.UpdateAsync(dto, id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return FromResponse(
                await _productBatchService.DeleteAsync( id));
        }
    }
}
