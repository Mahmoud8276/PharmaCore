using Mapster;
using Pharmacy.System.Core.Dtos;
using Pharmacy.System.Core.Dtos.InventoryItemDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using Pharmacy.System.Services.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> GetAllAsync(InventoryItemSpecParams specParams)
        {
            var countSpec = new InventoryItemSpecification(specParams, true);
            var count = await _unitOfWork.InventoryItemRepository
                .GetCountWithSpecAsync(countSpec);

            var spec = new InventoryItemSpecification(specParams);
            var data = await _unitOfWork.InventoryItemRepository
                .GetAllWithSpecAsync(spec);

            var pagination = new Pagination(
                specParams.PageIndex,
                specParams.PageSize,
                count, 
                data.Adapt<List<InventoryItemDetailsDto>>());

            return Response.Success(
                message: "Inventory items retreived successfully",
                data: pagination);
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.InventoryItemRepository
                .GetWithSpecAsync(new InventoryItemSpecification(id));
            if(item == null)
            {
                return Response.NotFound(
                    message: "Inventory item not found");
            }

            return Response.Success(
                message: "Inventory item retreived successfully",
                data: item.Adapt<InventoryItemDetailsDto>());
        }
    }
}
