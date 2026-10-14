using Pharmacy.System.Core.Dtos.SupplyDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public class SupplyService : ISupplyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SupplyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public Task<Response> CreateAsync(SupplyDto dto)
        {
            throw new global::System.NotImplementedException();
        }
        public Task<Response> GetByIdAsync(int id)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> GetAllAsync(SupplySpecParams specParams)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> UpdateAsync(int id, SupplyDto dto)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> DeleteAsync(int id)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> ConfirmAsync(int id)
        {
            throw new global::System.NotImplementedException();
        }

    }
}
