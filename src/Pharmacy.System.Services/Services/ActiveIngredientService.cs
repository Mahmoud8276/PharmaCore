using Pharmacy.System.Core.Dtos.ActiveIngredientDtos;
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
    public class ActiveIngredientService
        : WriteService<ActiveIngredient, int, ActiveIngredientSpecParams, ActiveIngredientDto, ActiveIngredientDto, ActiveIngredientDetailsDto>,
          IActiveIngredientService
    {
        public ActiveIngredientService(
            IGenericRepository<ActiveIngredient, int> repository, 
            IUnitOfWork unitOfWork) : base(repository, unitOfWork)
        {
        }

        protected override ISpecification<ActiveIngredient, int> BuildSpec(ActiveIngredientSpecParams specParams, bool isCountQuery)
        {
            return new ActiveIngredientSpecification(specParams, isCountQuery);
        }

        protected override async Task<Response> BeforeDeleteAsync(ActiveIngredient entity)
        {
            if(await _unitOfWork.ProductActiveIngredientRepository.AnyAsync(x => x.ActiveIngredientId == entity.Id))
            {
                return Response.Conflict(
                    message: "Cannot delete active ingredient because it is associated with one or more products.");
            }

            return Response.Success();
        }
    }
}
