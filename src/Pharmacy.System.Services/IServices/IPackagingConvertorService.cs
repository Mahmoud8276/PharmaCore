using Pharmacy.System.Core.Models;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface IPackagingConvertorService
    {
        public Task<int> ToBaseUnit(int productId, int PackagingUnitId, decimal quantity);
        public Task<int> ToBaseUnit(int PackagingLevelId, decimal quantity);
        public Task<int> ToBaseUnit(PackagingLevelSnapshot packagingSnapshot, decimal quantity);
    }
}
