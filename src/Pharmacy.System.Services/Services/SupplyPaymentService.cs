using Mapster;
using Pharmacy.System.Core.Dtos.SupplyPaymentDtos;
using Pharmacy.System.Core.Enums;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.Specifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public class SupplyPaymentService : ISupplyPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SupplyPaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        

        public async Task<Response> CreateAsync(int supplyId, SupplyPaymentDto dto)
        {
            var supply = await _unitOfWork.SupplyRepository
                .GetWithSpecAsync(new SupplySpecification(supplyId));
            if(supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }

            if (!supply.IsConfirmed)
            {
                return Response.Fail(
                    message: "Supply is not confirmed!");
            }

            if(supply.PaymentPlan != SupplyPaymentPlan.FreePayment)
            {
                return Response.Fail(
                    message: "Supply payment plan is not free payment!");
            }

            if(dto.Amount > supply.RemainingAmount)
            {
                return Response.Fail(
                    message: "The payment amount is larger than the supply remaining amount!",
                    details: $"Payment amount: {dto.Amount}, Supply remaining amount: {supply.RemainingAmount}");
            }

            var payment = dto.Adapt<SupplyPayment>();
            supply.Payments.Add(payment);
            supply.RecalculatePaymentStatus();

            await _unitOfWork.CompleteAsync();

            return Response.Created(
                message: "Supply payment created successfully",
                data: payment.Adapt<SupplyPaymentDetailsDto>());
        }

        public async Task<Response> DeleteAsync(int supplyId, int paymentId)
        {
            var supply = await _unitOfWork.SupplyRepository
                .GetWithSpecAsync(new SupplySpecification(supplyId));
            if(supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }

            if(supply.PaymentPlan != SupplyPaymentPlan.FreePayment)
            {
                return Response.Fail(
                    message: "Payment plan is not free payment!");
            }

            var payment = supply.Payments.FirstOrDefault(x=>x.Id == paymentId);
            if(payment == null)
            {
                return Response.NotFound(
                    message: "Payment not found!");
            }

            //Handle the case if there are supplier credits related to this supply
            var supplierCredits = await _unitOfWork.SupplierCreditRepository
                .GetUnSettledCreditsBySupplyIdAsync(supplyId);
            if(supplierCredits.Count() > 0)
            {
                if (supplierCredits.Sum(x => x.Amount) <= payment.Amount)
                {
                    _unitOfWork.SupplierCreditRepository.DeleteRange(supplierCredits);
                }
                else
                {
                    List<SupplierCredit> creditsToBeDeleted = new();
                    var amount = payment.Amount;
                    foreach (var credit in supplierCredits)
                    {
                        var rem = amount - credit.Amount;
                        if (rem > 0)
                        {
                            creditsToBeDeleted.Add(credit);
                            amount = rem;
                        }
                        else if (rem == 0)
                        {
                            creditsToBeDeleted.Add(credit);
                            break;
                        }
                        else
                        {
                            credit.Amount = rem * -1;
                            break;
                        }
                    }

                    if (creditsToBeDeleted.Count() > 0)
                        _unitOfWork.SupplierCreditRepository.DeleteRange(creditsToBeDeleted);
                }

            }

            supply.Payments.Remove(payment);
            _unitOfWork.SupplyPaymentRepository.Delete(payment);
            supply.RecalculatePaymentStatus();
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Payment deleted successfully");

        }

        public async Task<Response> GetAllBySupplyIdAsync(int supplyId)
        {
            var payments = await _unitOfWork.SupplyPaymentRepository
                .GetAllBySupplyId(supplyId);
            if(payments.Count == 0)
            {
                return Response.NotFound(
                    message: "No payments found for this supply!");
            }

            return Response.Success(
                message: "Payments retrieved successfully",
                data:  payments.Adapt<List<SupplyPaymentDetailsDto>>());
        }

        public async Task<Response> GetByIdAsync(int supplyId, int paymentId)
        {
            var payment = await _unitOfWork.SupplyPaymentRepository
                .GetByIdAsync(paymentId);
            if(payment == null || payment.SupplyId != supplyId)
            {
                return Response.NotFound(
                    message: "Payment not found!");
            }

            return Response.Success(
                message: "Payment retreived successfully",
                data: payment.Adapt<SupplyPaymentDetailsDto>());
        }

        public Task<Response> UpdateAsync(int supplyId, int paymentId, SupplyPaymentDto dto)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
