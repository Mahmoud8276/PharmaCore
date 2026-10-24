using Pharmacy.System.Core.Dtos.SupplyInstallmentDtos;
using Pharmacy.System.Services.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface ISupplyInstallmentService
    {
        public Task<Response> CreateInstallmentPlanAsync(int supplyId, List<SupplyInstallmentDto> dtos);
        public Task<Response> UpdateInstallmentPlanAsync(int supplyId, List<SupplyInstallmentDto> dtos);
        public Task<Response> GetInstallmentPlanAsync(int supplyId);
        public Task<Response> DeleteInstallmentPlanAsync(int supplyId);
        public Task<Response> DeleteInstallmentAsync(int supplyId, int installmentId);
        public Task<Response> PaySupplyInstallmentAsync(int supplyId, int installmentId, SupplyInstallmentPaymentDto dto);
    }
}
