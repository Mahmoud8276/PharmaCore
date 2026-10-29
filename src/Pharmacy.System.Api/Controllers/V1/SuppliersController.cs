using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.SupplierDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SuppliersController : BaseController
    {
        private readonly ISupplierService _supplierService;
        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] SupplierDto dto)
        {
            return FromResponse(
                await _supplierService.CreateAsync(dto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] SupplierSpecParams specParams)
        {
            return FromResponse(
                await _supplierService.GetAllAsync(specParams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return FromResponse(
                await _supplierService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateSupplierDto dto)
        {
            return FromResponse(
                await _supplierService.UpdateAsync(id, dto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return FromResponse(
                await _supplierService.DeleteAsync(id));
        }

        [HttpPut("{id}/logo")]
        public async Task<IActionResult> UpdateLogoAsync(int id, [FromForm] UpdateSupplierLogoDto dto)
        {
            return FromResponse(
                await _supplierService.UpdateLogoAsync(id, dto));
        }

        [HttpDelete("{id}/logo")]
        public async Task<IActionResult> DeleteLogoAsync(int id)
        {
            return FromResponse(
                await _supplierService.DeleteLogoAsync(id));
        }
    }
}
