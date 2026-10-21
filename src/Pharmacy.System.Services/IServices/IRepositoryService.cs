using Pharmacy.System.Core.Dtos.RepositoryDtos;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.SpecificationParams;

namespace Pharmacy.System.Services.IServices
{
    public interface IRepositoryService 
        : IWriteService<Repository, int, RepositoryDto, RepositoryDto, RepositorySpecParams>
    {
    }
}
