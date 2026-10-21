using Pharmacy.System.Core.Dtos.SupplyPaymentDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
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

        
        public Task<Response> CreateAsync(int supplyId, SupplyPaymentDto dto)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> DeleteAsync(int supplyId, int id)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> GetAllBySupplyIdAsync(int supplyId)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> GetByIdAsync(int supplyId, int id)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> UpdateAsync(int supplyId, int id, SupplyPaymentDto dto)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
