using Pharmacy.System.Core.Dtos.ProductBatchDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using Pharmacy.System.Services.Specifications;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public class ProductBatchService
        : WriteService<ProductBatch, int, ProductBatchSpecParams, ProductBatchDto, ProductBatchDto, ProductBatchDetailsDto>,
        IProductBatchService
    {
        public ProductBatchService(
            IGenericRepository<ProductBatch, int> repository,
            IUnitOfWork unitOfWork)
            : base(repository, unitOfWork) { }

        protected override ISpecification<ProductBatch, int> BuildSpec(ProductBatchSpecParams specParams, bool isCountQuery)
        {
            return new ProductBatchSpecification(specParams, isCountQuery);
        }

        protected override async Task<Response> BeforeCreateAsync(ProductBatch entity, ProductBatchDto dto)
        {
            //Check if the number already exists or not
            if(await _unitOfWork.ProductBatchRepository
                .AnyAsync(x=>x.Number == dto.Number))
            {
                return Response.Conflict(
                    message: "Product batch number already exist!");
            }

            return Response.Success();
        }

        protected override async Task<Response> BeforeUpdateAsync(ProductBatch entity, ProductBatchDto dto)
        {
            //Check if the number already exists or not
            if (await _unitOfWork.ProductBatchRepository
                .AnyAsync(x => x.Id != entity.Id && x.Number == dto.Number))
            {
                return Response.Conflict(
                    message: "Product batch number already exist!");
            }

            return Response.Success();
        }

        protected override async Task<Response> BeforeDeleteAsync(ProductBatch entity)
        {
            //Chek if the product batch is associated with any supply items
            if(await _unitOfWork.SupplyItemRepository
                .AnyAsync(x=>x.ProductBatchId == entity.Id))
            {
                return Response.Conflict(
                    message: "Can not delete product batch!",
                    details: "Product batch is associated with existing supply items.");
            }
            
            //Chek if the product batch is associated with any inventory items
            if(await _unitOfWork.InventoryItemRepository
                .AnyAsync(x=>x.ProductBatchId == entity.Id))
            {
                return Response.Conflict(
                    message: "Can not delete product batch!",
                    details: "Product batch is associated with existing inventory items.");
            }

            return Response.Success();
        }
    }
}
