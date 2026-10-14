using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.SupplyDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SupplyController : BaseController
    {
        private readonly ISupplyService _supplyService;
        public SupplyController(ISupplyService supplyService)
        {
            _supplyService = supplyService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] SupplyDto dto)
        {
            var response = await _supplyService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] SupplySpecParams specParams)
        {
            var response = await _supplyService.GetAllAsync(specParams);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var response = await _supplyService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] SupplyDto dto)
        {
            var response = await _supplyService.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _supplyService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmAsync(int id)
        {
            var response = await _supplyService.ConfirmAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
