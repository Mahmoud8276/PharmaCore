using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.ProductCategoryDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class ProductCategoriesController : BaseController
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoriesController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] ProductCategoryDto dto)
        {
            return FromResponse(
                await _productCategoryService.CreateAsync(dto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductCategorySpecParams specParams)
        {
            return FromResponse(
                await _productCategoryService.GetAllAsync(specParams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return FromResponse(
                await _productCategoryService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateProductCategoryDto dto)
        {
            return FromResponse(
                await _productCategoryService.UpdateAsync(dto, id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return FromResponse(
                await _productCategoryService.DeleteAsync(id));
        }

        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateCategoryImageAsync(int id, [FromForm] UpdateCategoryImageDto dto)
        {
            return FromResponse(
                await _productCategoryService.UpdateCategoryImageAsync(id, dto));
        }

        [HttpDelete("{id}/image")]
        public async Task<IActionResult> DeleteCategoryImageAsync(int id)
        {
            return FromResponse(
                await _productCategoryService.DeleteCategoryImageAsync(id));
        }
    }
}
