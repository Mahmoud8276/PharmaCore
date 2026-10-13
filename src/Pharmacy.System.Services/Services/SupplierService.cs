using Pharmacy.System.Core.Dtos.SupplierDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public Task<Response> CreateAsync(SupplierDto dto)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> DeleteAsync(int id)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> GetAllAsync(SupplierSpecParams specParams)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> GetByIdAsync(int id)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<Response> UpdateAsync(int id, SupplierDto dto)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
