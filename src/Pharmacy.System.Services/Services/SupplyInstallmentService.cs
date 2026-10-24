using Pharmacy.System.Core.Dtos.SupplyInstallmentDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using System.Collections.Generic;
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

        public Task<Response> CreateInstallmentPlanAsync(int supplyId, List<SupplyInstallmentDto> dtos)
        {
            throw new global::System.NotImplementedException();
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
