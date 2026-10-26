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

        public async Task<Response> CreateAsync(int supplyId, List<SupplyInstallmentDto> dtos)
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

            //Check that the supply payment plan is paying by Installments.
            if (supply.PaymentPlan != SupplyPaymentPlan.Installment)
            {
                return Response.Fail(
                    message: "Supply payment plan is not Installments.");
            }

            //Ckeck that the total amount of the installments is
            //not greater than the remaining amount of the supply.
            if(supply.Installments.Sum(x => x.Amount)
                + dtos.Sum(x => x.Amount) > supply.RemainingAmount)
            {
                return Response.Fail(
                    message: "Total amount of installments is greater than the remaining amount of the supply.");
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

        public async Task<Response> DeleteAsync(int supplyId, int installmentId)
        {
            var installment = await _unitofWork.SupplyInstallmentRepository
                .GetWithSpecAsync(new SupplyInstallmentSpecification(installmentId));
            if(installment == null || installment.SupplyId != supplyId)
            {
                return Response.NotFound(
                    message: "Supply installment not found!");
            }

            if(installment.Payments.Count() > 0)
            {
                return Response.Conflict(
                    message: "Installment is associated with payments!");
            }

            _unitofWork.SupplyInstallmentRepository.Delete(installment);
            await _unitofWork.CompleteAsync();

            return Response.Success(
                message: "Installment deleted successfully");
        }

        public async Task<Response> DeleteAsync(int supplyId)
        {
            if(!await _unitofWork.SupplyRepository.AnyAsync(x=>x.Id == supplyId))
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }

            var installments = await _unitofWork.SupplyInstallmentRepository
                .GetSupplyInstallmentsAsync(supplyId);
            if(installments.Count() == 0)
            {
                return Response.Fail(
                    message: "No installment configured for this supply!");
            }

            var paidInstallmentIds = installments
                .Where(x => x.Payments.Count > 0)
                .Select(x=>x.Id);
            if(paidInstallmentIds.Count() > 0)
            {
                return Response.Conflict(
                    message: "There are payments associated with one or more installments!",
                    details: $"installments with Ids {string.Join(", ", paidInstallmentIds)} are associated with payments!");
            }

            _unitofWork.SupplyInstallmentRepository
                .DeleteRange(installments);
            await _unitofWork.CompleteAsync();

            return Response.Success(
                message: "Supply installments deleted successfully");
        }

        public async Task<Response> GetAsync(int supplyId)
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
                .GetSupplyInstallmentsAsync(supplyId);

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

        public Task<Response> GetAsync(int supplyId, int installmentId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> PayInstallmentAsync(int supplyId, int installmentId, SupplyInstallmentPaymentDto dto)
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

        public async Task<Response> UpdateAsync(int supplyId, int installmentId, SupplyInstallmentDto dto)
        {
            if(dto.DueDate < DateOnly.FromDateTime(DateTime.Now))
            {
                return Response.Fail(
                    message: "Due date can not be in the past!");
            }

            var supply = await _unitofWork.SupplyRepository.GetWithSpecAsync(
                new SupplySpecification(supplyId));
            if(supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }

            var installment = supply.Installments.FirstOrDefault(x => x.Id == installmentId);
            if (installment == null)
            {
                return Response.NotFound(
                    message: "Installment not found!");
            }

            if(dto.Amount < installment.PaidAmount)
            {
                return Response.Fail(
                    message: "Installment Amount can not be less than the already paid amount for that installment!");
            }

            dto.Adapt(installment);
            var installmentsSum = supply.Installments.Sum(x => x.Amount);
            if (installmentsSum > supply.RemainingAmount)
            {
                return Response.Fail(
                    message: "Total amount of installments is greater than the remaining amount of the supply.",
                    details: $"Total installments amount: {installmentsSum}, Supply remaining amount: {supply.RemainingAmount}");
            }

            if(installment.PaidAmount == dto.Amount) 
                installment.Status = SupplyInstallmentStatus.Paid;

            await _unitofWork.CompleteAsync();

            return Response.Success(
                message: "Installment updated successfully",
                data: installment.Adapt<SupplyInstallmentDetailsDto>());
        }
    }
}
    