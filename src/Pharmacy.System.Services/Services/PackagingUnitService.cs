using Pharmacy.System.Core.Dtos.PackagingUnitDtos;
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
    public class PackagingUnitService :
        WriteService<PackagingUnit, int, PackagingUnitSpecParams, PackagingUnitDto, PackagingUnitDto, PackagingUnitDetailsDto>,
        IPackagingUnitService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PackagingUnitService(
            IGenericRepository<PackagingUnit, int> repository, 
            IUnitOfWork unitOfWork) 
            : base(repository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override ISpecification<PackagingUnit, int> BuildSpec(PackagingUnitSpecParams specParams, bool isCountQuery)
        {
            return new PackagingUnitSpecification(specParams, isCountQuery);
        }

        override protected async Task<Response> BeforeDeleteAsync(PackagingUnit entity)
        {
            if (await _unitOfWork.ProductPackagingLevelRepository.AnyAsync(x => x.PackagingUnitId == entity.Id))
            {
                return Response.Conflict(
                    message: "Cannot delete packaging unit because it is associated with one or more products.");
            }
            return Response.Success();
        }
    }
}
