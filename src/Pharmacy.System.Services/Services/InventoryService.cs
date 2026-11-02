using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Response> GetAllAsync(InventorySpecParams specParams)
        {
            var pagination = await _unitOfWork.InventoryRepository
                .GetAllInventoryGroupedByProduct(specParams);

            return Response.Success(
                message: "Inventory retrieved successfully",
                data: pagination);
        }
    }
}
