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
            return FromResponse(
                await _packagingUnitService.CreateAsync(packagingUnitDto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PackagingUnitSpecParams specParams)
        {
            return FromResponse(
                await _packagingUnitService.GetAllAsync(specParams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return FromResponse(
                await _packagingUnitService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] PackagingUnitDto packagingUnitDto)
        {
            return FromResponse(
                await _packagingUnitService.UpdateAsync(packagingUnitDto, id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return FromResponse(
                await _packagingUnitService.DeleteAsync(id));
        }
    }
}
