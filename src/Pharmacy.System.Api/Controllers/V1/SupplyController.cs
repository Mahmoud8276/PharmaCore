using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.SupplyDtos;
using Pharmacy.System.Core.Dtos.SupplyPaymentDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SupplyController : BaseController
    {
        private readonly ISupplyService _supplyService;
        private readonly ISupplyPaymentService _supplyPaymentService;
        public SupplyController(
            ISupplyService supplyService,
            ISupplyPaymentService supplyPaymentService)
        {
            _supplyService = supplyService;
            _supplyPaymentService = supplyPaymentService;
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
        public async Task<IActionResult> ConfirmAsync(int id, [FromQuery] SupplyPaymentConfirmationDto dto)
        {
            var response = await _supplyService.ConfirmAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{supplyId}/payments")]
        public async Task<IActionResult> CreatePaymentAsync(int supplyId, [FromBody] SupplyPaymentDto dto)
        {
            var response = await _supplyPaymentService.CreateAsync(supplyId, dto);
            return StatusCode(response.StatusCode, response);
        }
        
        [HttpGet("{supplyId}/payments")]
        public async Task<IActionResult> GetAllPaymentsAsync(int supplyId)
        {
            var response = await _supplyPaymentService.GetAllBySupplyIdAsync(supplyId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{supplyId}/payments/{id}")]
        public async Task<IActionResult> GetPaymentByIdAsync(int supplyId, int id)
        {
            var response = await _supplyPaymentService.GetByIdAsync(supplyId, id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{supplyId}/payments/{id}")]
        public async Task<IActionResult> UpdatePaymentAsync(int supplyId, int id, [FromBody] SupplyPaymentDto dto)
        {
            var response = await _supplyPaymentService.UpdateAsync(supplyId, id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{supplyId}/payments/{id}")]
        public async Task<IActionResult> DeletePaymentAsync(int supplyId, int id)
        {
            var response = await _supplyPaymentService.DeleteAsync(supplyId, id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
