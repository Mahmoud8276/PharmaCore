using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Api.Responses;
using Pharmacy.System.Core.Dtos.SupplyDtos;
using Pharmacy.System.Core.Dtos.SupplyInstallmentDtos;
using Pharmacy.System.Core.Dtos.SupplyPaymentDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SupplyController : BaseController
    {
        private readonly ISupplyService _supplyService;
        private readonly ISupplyPaymentService _supplyPaymentService;
        private readonly ISupplyInstallmentService _supplyInstallmentService;
        public SupplyController(
            ISupplyService supplyService,
            ISupplyPaymentService supplyPaymentService,
            ISupplyInstallmentService supplyInstallmentService)
        {
            _supplyService = supplyService;
            _supplyPaymentService = supplyPaymentService;
            _supplyInstallmentService = supplyInstallmentService;
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



        [HttpPost("{supplyId}/installments")]
        public async Task<IActionResult> CreateInstallmentPlanAsync(int supplyId, [FromBody] List<SupplyInstallmentDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
            {
                return BadRequest(ApiResponse.Fail(
                    message : "Installment plan can not be empty!"));
            }

            var response = await _supplyInstallmentService.CreateAsync(supplyId, dtos);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{supplyId}/installments")]
        public async Task<IActionResult> GetInstallmentPlanAsync(int supplyId)
        {
            var response = await _supplyInstallmentService.GetAsync(supplyId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{supplyId}/installments/{installmentId}")]
        public async Task<IActionResult> GetInstallmentAsync(int supplyId, int installmentId)
        {
            var response = await _supplyInstallmentService.GetAsync(supplyId, installmentId);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPut("{supplyId}/installments/{installmentId}")]
        public async Task<IActionResult> UpdateInstallmentAsync(
            int supplyId,
            int installmentId, 
            [FromBody] SupplyInstallmentDto dto)
        {
            var response = await _supplyInstallmentService.UpdateAsync(supplyId, installmentId, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{supplyId}/installments")]
        public async Task<IActionResult> DeleteInstallmentPlanAsync(int supplyId)
        {
            var response = await _supplyInstallmentService.DeleteAsync(supplyId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{supplyId}/installments/{installmentId}")]
        public async Task<IActionResult> DeleteInstallmentAsync(int supplyId, int installmentId)
        {
            var response = await _supplyInstallmentService.DeleteAsync(supplyId, installmentId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{supplyId}/installments/{installmentId}/pay")]
        public async Task<IActionResult> PaySupplyInstallmentAsync(
            int supplyId, 
            int installmentId, 
            [FromBody] SupplyInstallmentPaymentDto dto)
        {
            var response = await _supplyInstallmentService
                .PayInstallmentAsync(supplyId, installmentId, dto);
            return StatusCode(response.StatusCode, response);
        }
    }
}
