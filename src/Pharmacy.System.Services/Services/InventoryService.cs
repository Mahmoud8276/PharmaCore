using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using Pharmacy.System.Services.Specifications;
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
                .GetAllInventoryDetailsWithSpecAsync(specParams);

            return Response.Success(
                message: "Inventory retrieved successfully",
                data: pagination);
        }

        public async Task<Response> GetByProductIdAsync(int productId)
        {
            //Check if the product exists or not
            var product = await _unitOfWork.ProductRepository
                .GetWithSpecAsync(new ProductSpecification(productId));
            if(product == null)
            {
                return Response.NotFound(
                    message: "Product not found");
            }

            return Response.Success(
                message: "Product stock details retrieved successfully",
                data: await _unitOfWork.InventoryRepository
                .GetProductStockDetailsAsync(product));
        }

        public async Task<Response> GetAllProductStockGroupsAsync(int productId, ProductStockGroupSpecParams specParams)
        {
            //Check if the product exists or not
            var product = await _unitOfWork.ProductRepository
                .GetWithSpecAsync(new ProductSpecification(productId));
            if (product == null)
            {
                return Response.NotFound(
                    message: "Product not found");
            }

            var result = await _unitOfWork.InventoryRepository
                .GetProductStockGroupDetailsWithSpecAsync(product, specParams);
            return Response.Success(
                message: "Product stock details retrieved successfully",
                data: result);
        }

    }
}
