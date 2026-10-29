using Pharmacy.System.Core.Dtos.SupplyInstallmentDtos;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface ISupplyInstallmentService
    {
        public Task<Response> CreateAsync(int supplyId, List<SupplyInstallmentDto> dtos);
        public Task<Response> UpdateAsync(int supplyId, int installmentId, SupplyInstallmentDto dto);
        public Task<Response> GetBySupplyIdAsync(int supplyId);
        public Task<Response> GetByIdAsync(int supplyId, int installmentId);
        public Task<Response> GetAllAsync(SupplyInstallmentSpecParams specParams);
        public Task<Response> DeleteAsync(int supplyId);
        public Task<Response> DeleteAsync(int supplyId, int installmentId);


        public Task<Response> CreateInstallmentPaymentAsync(int supplyId, int installmentId, SupplyInstallmentPaymentDto dto);
    }
}
