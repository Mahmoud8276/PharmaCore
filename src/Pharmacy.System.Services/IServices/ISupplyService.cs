using Pharmacy.System.Core.Dtos.SupplyDtos;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface ISupplyService
    {
        public Task<Response> CreateAsync(SupplyDto dto);
        public Task<Response> GetByIdAsync(int id);
        public Task<Response> GetAllAsync(SupplySpecParams specParams);
        public Task<Response> UpdateAsync(int id, SupplyDto dto);
        public Task<Response> DeleteAsync(int id);
        public Task<Response> ConfirmAsync(int id);
    }
}
