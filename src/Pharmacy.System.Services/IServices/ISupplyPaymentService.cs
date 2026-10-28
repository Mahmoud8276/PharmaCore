using Pharmacy.System.Core.Dtos.SupplyPaymentDtos;
using Pharmacy.System.Services.Responses;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface ISupplyPaymentService
    {
        public Task<Response> CreateAsync(int supplyId, SupplyPaymentDto dto);
        public Task<Response> UpdateAsync(int supplyId, int paymentId, UpdateSupplyPaymentDto dto);
        public Task<Response> DeleteAsync(int supplyId, int paymentId);
        public Task<Response> GetByIdAsync(int supplyId, int paymentId);
        public Task<Response> GetAllBySupplyIdAsync(int supplyId);
    }
}
