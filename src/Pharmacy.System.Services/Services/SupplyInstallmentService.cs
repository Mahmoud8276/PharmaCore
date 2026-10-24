using Mapster;
using Pharmacy.System.Core.Dtos.SupplyInstallmentDtos;
using Pharmacy.System.Core.Enums;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public class SupplyInstallmentService : ISupplyInstallmentService
    {
        private readonly IUnitOfWork _unitofWork;
        public SupplyInstallmentService(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public async Task<Response> CreateInstallmentPlanAsync(int supplyId, List<SupplyInstallmentDto> dtos)
        {
            //Check that the supply exists.
            var supply = await _unitofWork.SupplyRepository.GetWithSpecAsync(
                new SupplySpecification(supplyId));
            if (supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found.");
            }

            //Check if the supply is confirmed or not
            if (!supply.IsConfirmed)
            {
                return Response.Fail(
                    message: "Supply is not confirmed, confirm it first and try again.");
            }

            //Check if there is already a supply installment plan for this supply.
            if (supply.Installments != null && supply.Installments.Count > 0)
            {
                return Response.Conflict(
                    message: "Supply installment plan already exists.");
            }

            //Check that the supply payment plan is paying by Installments.
            if (supply.PaymentPlan != SupplyPaymentPlan.Installment)
            {
                return Response.Fail(
                    message: "Supply payment method is not Installments.");
            }

            //Check that the supply is not already paid.
            if(supply.PaymentStatus != PaymentStatus.NotPaid)
            {
                return Response.Fail(
                    message: "Supply is partially or fully paid.");
            }

            //Ckeck that the total amount of the installments is equal to the remaining amount of the supply.
            if(dtos.Sum(x => x.Amount) != supply.RemainingAmount)
            {
                return Response.Fail(
                    message: "Total amount of installments is not equal to the remaining amount of the supply.");
            }

            //Check that the installments due dates are not in the past.
            if (dtos.Any(x => x.DueDate < DateOnly.FromDateTime(DateTime.UtcNow)))
            {
                return Response.Fail(
                    message: "Installment due dates cannot be in the past.");
            }

            supply.Installments.AddRange(dtos.Adapt<List<SupplyInstallment>>());
            await _unitofWork.CompleteAsync();

            var result = new List<SupplyInstallmentDetailsDto>();
            try
            {
                result = supply.Installments
                    .Adapt<List<SupplyInstallmentDetailsDto>>();
            }
            catch (Exception ex) { }

            return Response.Success(
                message: "Supply installment created successfully",
                data: result);
        }

        public Task<Response> DeleteInstallmentAsync(int supplyId, int installmentId)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> DeleteInstallmentPlanAsync(int supplyId)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> GetInstallmentPlanAsync(int supplyId)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> UpdateInstallmentPlanAsync(int supplyId, List<SupplyInstallmentDto> dtos)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
