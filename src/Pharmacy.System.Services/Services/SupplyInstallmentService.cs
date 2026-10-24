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
                    message: "Supply payment plan is not Installments.");
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

            var result = supply.Installments
                .OrderBy(x => x.DueDate)
                .ToList()
                .Adapt<List<SupplyInstallmentDetailsDto>>();

            return Response.Created(
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

        public async Task<Response> GetInstallmentPlanAsync(int supplyId)
        {
            var supply = await _unitofWork.SupplyRepository
                .GetByIdAsync(supplyId);
            if(supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }

            if (!supply.IsConfirmed)
            {
                return Response.Fail(
                    message: "Supply is not confirmed yet!");
            }

            if(supply.PaymentPlan != SupplyPaymentPlan.Installment)
            {
                return Response.Fail(
                    message: "Supply payment plan is not Installments.");
            }

            var installments = await _unitofWork.SupplyInstallmentRepository
                .GetSupplyInstallments(supplyId);

            if(installments == null || installments.Count == 0)
            {
                return Response.NotFound(
                    message: "There are no installments configured for this supply yet!");
            }

            var result = installments.Adapt<List<SupplyInstallmentDetailsDto>>()
                .OrderBy(x => x.DueDate);
            return Response.Success(
                message: "Supply installment retreived successfully",
                data: result);
        }

        public async Task<Response> PaySupplyInstallmentAsync(int supplyId, int installmentId, SupplyInstallmentPaymentDto dto)
        {
            //Check if the supply exists or not.
            var supply = await _unitofWork.SupplyRepository
                .GetWithSpecAsync(new SupplySpecification(supplyId));
            if(supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }

            //Check if the installment exists and belongs to this supply or not.
            var installment = supply.Installments.FirstOrDefault(x => x.Id == installmentId);
            if(installment == null)
            {
                return Response.NotFound(
                    message: "Installment not found!");
            }

            //Check if the supply is confirmed or not.
            if (!supply.IsConfirmed)
            {
                return Response.Fail(
                    message: "Supply is not confirmed, confirm it first and try again.");
            }

            //Check if the supply is fully paid or not.
            if(supply.PaymentStatus == PaymentStatus.FullyPaid)
            {
                return Response.Fail(
                    message: "Supply is already paid!");
            }

            //Check if the supply remaining amount is larger than the payment amount or not.
            if(dto.Amount > supply.RemainingAmount)
            {
                return Response.Fail(
                    message: "Payment amount should not exceed supply remaining amount!",
                    details: $"payment amount: {dto.Amount}, supply remaining amount: {supply.RemainingAmount}");
            }

            //Check if the installment is fully paid or not.
            if(installment.Status == SupplyInstallmentStatus.Paid)
            {
                return Response.Fail(
                    message: "Installment is already paid!");
            }

            //Check if the remaining installment amount is larger than or equal to the payment amount or not.
            if(dto.Amount > installment.RemainingAmount)
            {
                return Response.Fail(
                    message: "Payment amount should not exceed installment remaining amount",
                    details: $"Payment amount: {dto.Amount}, installment remaining amount: {installment.RemainingAmount}");
            }

            var newPayment = dto.Adapt<SupplyPayment>();
            newPayment.Supply = supply;
            newPayment.SupplyInstallment = installment;

            installment.Payments.Add(newPayment);
            installment.Status = installment.RemainingAmount == 0 ?
                SupplyInstallmentStatus.Paid : SupplyInstallmentStatus.PartiallyPaid;

            supply.PaymentStatus = supply.RemainingAmount == 0 ?
                PaymentStatus.FullyPaid : PaymentStatus.PartiallyPaid;

            await _unitofWork.CompleteAsync();

            var result = supply.Installments
                .OrderBy(x => x.DueDate)
                .ToList()
                .Adapt<List<SupplyInstallmentDetailsDto>>();
            return Response.Created(
                message: "Supply installment paid successfully",
                data: result);
        }

        public Task<Response> UpdateInstallmentPlanAsync(int supplyId, List<SupplyInstallmentDto> dtos)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
