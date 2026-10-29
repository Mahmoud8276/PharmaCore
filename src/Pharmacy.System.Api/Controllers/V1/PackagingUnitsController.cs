using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.PackagingUnitDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class PackagingUnitsController :BaseController
    {
        private readonly IPackagingUnitService _packagingUnitService;
        public PackagingUnitsController(IPackagingUnitService packagingUnitService)
        {
            _packagingUnitService = packagingUnitService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] PackagingUnitDto packagingUnitDto)
        {
            var result = await _packagingUnitService.CreateAsync(packagingUnitDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PackagingUnitSpecParams specParams)
        {
            var result = await _packagingUnitService.GetAllAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _packagingUnitService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] PackagingUnitDto packagingUnitDto)
        {
            var result = await _packagingUnitService.UpdateAsync(packagingUnitDto, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _packagingUnitService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
