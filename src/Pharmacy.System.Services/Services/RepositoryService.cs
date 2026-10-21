using Pharmacy.System.Core.Dtos.RepositoryDtos;
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
    public class RepositoryService :
        WriteService<Repository, int, RepositorySpecParams, RepositoryDto, RepositoryDto, RepositoryDetailsDto>
        , IRepositoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RepositoryService(
            IGenericRepository<Repository, int> repository,
            IUnitOfWork unitOfWork) 
            : base(repository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override ISpecification<Repository, int> BuildSpec(RepositorySpecParams specParams, bool isCountQuery)
        {
            return new RepositorySpecification(specParams, isCountQuery);
        }

        protected override async Task<Response> BeforeCreateAsync(Repository entity, RepositoryDto dto)
        {
            //Check that the name dose not already exist
            if (await _unitOfWork.RepositoryRepository.AnyAsync(x=>x.Name == dto.Name))
            {
                return Response.Conflict(
                    message: "Repository Name already exists!");
            }

            return Response.Success();
        }

        protected override async Task<Response> BeforeUpdateAsync(Repository entity, RepositoryDto dto)
        {
            //Check that the name dose not already exist
            if (await _unitOfWork.RepositoryRepository.AnyAsync(
                x => x.Id != entity.Id && x.Name == dto.Name))
            {
                return Response.Conflict(
                    message: "Repository Name already exists!");
            }

            return Response.Success();
        }

        protected override async Task<Response> BeforeDeleteAsync(Repository entity)
        {
            //Check there is no supplies received from this repository before deleting it
            if(await _unitOfWork.SupplyRepository.AnyAsync(x=>x.RepositoryId == entity.Id))
            {
                return Response.Conflict(
                    message: "Cannot delete repository because there are supplies received in it!");
            }

            //TODO: Check there is no inventory items inside it

            return Response.Success();
        }
    }
}
