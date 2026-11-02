using Pharmacy.System.Core.Dtos;
using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Core.Interfaces.IRepositories
{
    public interface IInventoryRepository
    {
        public Task<Pagination> GetAllInventoryGroupedByProduct(IInventorySpecParams specParams);
    }
}
