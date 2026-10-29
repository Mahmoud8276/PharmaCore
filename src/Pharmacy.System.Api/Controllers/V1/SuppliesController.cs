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
    public class SuppliesController : BaseController
    {
        private readonly ISupplyService _supplyService;
        private readonly ISupplyPaymentService _supplyPaymentService;
        private readonly ISupplyInstallmentService _supplyInstallmentService;
        public SuppliesController(
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
            return FromResponse(await _supplyService.CreateAsync(dto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] SupplySpecParams specParams)
        {
            return FromResponse(await _supplyService.GetAllAsync(specParams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return FromResponse(await _supplyService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] SupplyDto dto)
        {
            return FromResponse(await _supplyService.UpdateAsync(id, dto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return FromResponse(await _supplyService.DeleteAsync(id));
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmAsync(int id, [FromBody] SupplyPaymentConfirmationDto dto)
        {
            return FromResponse(await _supplyService.ConfirmAsync(id, dto));
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetAllPaymentsAsync(
            [FromQuery] SupplyPaymentSpecParams specParams)
        {
            return FromResponse(
                await _supplyPaymentService.GetAllAsync(specParams));
        }




        [HttpPost("{supplyId}/payments")]
        public async Task<IActionResult> CreatePaymentAsync(int supplyId, [FromBody] SupplyPaymentDto dto)
        {
            return FromResponse(await _supplyPaymentService.CreateAsync(supplyId, dto));
        }

        [HttpGet("{supplyId}/payments")]
        public async Task<IActionResult> GetAllSupplyPaymentsAsync(int supplyId)
        {
            return FromResponse(
                await _supplyPaymentService.GetAllBySupplyIdAsync(supplyId));
        }

        [HttpGet("{supplyId}/payments/{paymentId}")]
        public async Task<IActionResult> GetPaymentByIdAsync(int supplyId, int paymentId)
        {
            return FromResponse(
                await _supplyPaymentService.GetByIdAsync(supplyId, paymentId));
        }

        [HttpPatch("{supplyId}/payments/{paymentId}")]
        public async Task<IActionResult> UpdatePaymentAsync(int supplyId, int paymentId, [FromBody] UpdateSupplyPaymentDto dto)
        {
            return FromResponse(
                await _supplyPaymentService.UpdateAsync(supplyId, paymentId, dto));
        }

        [HttpDelete("{supplyId}/payments/{paymentId}")]
        public async Task<IActionResult> DeletePaymentAsync(int supplyId, int paymentId)
        {
            return FromResponse(await _supplyPaymentService.DeleteAsync(supplyId, paymentId));
        }






        [HttpPost("{supplyId}/installments")]
        public async Task<IActionResult> CreateInstallmentPlanAsync(int supplyId, [FromBody] List<SupplyInstallmentDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
            {
                return FromResponse(ApiResponse.Fail(
                    message: "Installment plan can not be empty!"));
            }

            return FromResponse(await _supplyInstallmentService.CreateAsync(supplyId, dtos));
        }

        [HttpGet("{supplyId}/installments")]
        public async Task<IActionResult> GetInstallmentPlanAsync(int supplyId)
        {
            return FromResponse(await _supplyInstallmentService.GetAsync(supplyId));
        }

        [HttpGet("{supplyId}/installments/{installmentId}")]
        public async Task<IActionResult> GetInstallmentAsync(int supplyId, int installmentId)
        {
            return FromResponse(
                await _supplyInstallmentService.GetAsync(supplyId, installmentId));
        }


        [HttpPut("{supplyId}/installments/{installmentId}")]
        public async Task<IActionResult> UpdateInstallmentAsync(
            int supplyId,
            int installmentId,
            [FromBody] SupplyInstallmentDto dto)
        {
            return FromResponse(
                await _supplyInstallmentService.UpdateAsync(supplyId, installmentId, dto));
        }

        [HttpDelete("{supplyId}/installments")]
        public async Task<IActionResult> DeleteInstallmentPlanAsync(int supplyId)
        {
            return FromResponse(
                await _supplyInstallmentService.DeleteAsync(supplyId));
        }

        [HttpDelete("{supplyId}/installments/{installmentId}")]
        public async Task<IActionResult> DeleteInstallmentAsync(int supplyId, int installmentId)
        {
            return FromResponse(
                await _supplyInstallmentService.DeleteAsync(supplyId, installmentId));
        }

        [HttpPost("{supplyId}/installments/{installmentId}/payments")]
        public async Task<IActionResult> CreateInstallmentPaymentAsync(
            int supplyId,
            int installmentId,
            [FromBody] SupplyInstallmentPaymentDto dto)
        {
            return FromResponse(
                await _supplyInstallmentService
                .CreateInstallmentPaymentAsync(supplyId, installmentId, dto));
        }
    }
}
