using Pharmacy.System.Core.Dtos.ProductFormDtos;
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
    public class ProductFormService :
        WriteService<ProductForm, int, ProductFormSpecParams, ProductFormDto, ProductFormDto, ProductFormDetailsDto>,
        IProductFormService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductFormService(
            IGenericRepository<ProductForm, int> repository,
            IUnitOfWork unitOfWork) 
            : base(repository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override ISpecification<ProductForm, int> BuildSpec(ProductFormSpecParams specParams, bool isCountQuery)
        {
            return new ProductFormSpecification(specParams, isCountQuery);
        }

        protected override async Task<Response> BeforeDeleteAsync(ProductForm entity)
        {
            if(await _unitOfWork.ProductRepository.AnyAsync(x=>x.ProductFormId == entity.Id))
            {
                return Response.Conflict(
                    message: "Cannot delete product form because it is associated with existing products.");
            }

            return Response.Success();
        }

        protected override async Task<Response> BeforeCreateAsync(ProductForm entity, ProductFormDto dto)
        {
            if(!await _unitOfWork.BaseUnitRepository.AnyAsync(x=>x.Id == dto.BaseUnitId))
            {
                return Response.NotFound(
                    message: "Base unit not found!");
            }

            return Response.Success();
        }

        protected override Task<Response> BeforeUpdateAsync(ProductForm entity, ProductFormDto dto)
        {
            return BeforeCreateAsync(entity, dto);
        }

    }
}
