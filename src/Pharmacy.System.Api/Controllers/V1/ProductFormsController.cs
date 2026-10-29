using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.ProductFormDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class ProductFormsController : BaseController
    {
        private readonly IProductFormService _productFormService;
        public ProductFormsController(IProductFormService productFormService)
        {
            _productFormService = productFormService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProductFormDto dto)
        {
            return FromResponse(
                await _productFormService.CreateAsync(dto));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductFormSpecParams specParams)
        {
            return FromResponse(
                await _productFormService.GetAllAsync(specParams));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return FromResponse(
                await _productFormService.GetByIdAsync(id));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProductFormDto dto)
        {
            return FromResponse(
                await _productFormService.UpdateAsync(dto, id));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return FromResponse(
                await _productFormService.DeleteAsync(id));
        }
    }
}
