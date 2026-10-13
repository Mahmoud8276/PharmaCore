using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.SupplierDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SupplierController : BaseController
    {
        private readonly ISupplierService _supplierService;
        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] SupplierDto dto)
        {
            var response = await _supplierService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] SupplierSpecParams specParams)
        {
            var response = await _supplierService.GetAllAsync(specParams);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var response = await _supplierService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateSupplierDto dto)
        {
            var response = await _supplierService.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _supplierService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}/logo")]
        public async Task<IActionResult> UpdateLogoAsync(int id, [FromForm] UpdateSupplierLogoDto dto)
        {
            var response = await _supplierService.UpdateLogoAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}/logo")]
        public async Task<IActionResult> DeleteLogoAsync(int id)
        {
            var response = await _supplierService.DeleteLogoAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
