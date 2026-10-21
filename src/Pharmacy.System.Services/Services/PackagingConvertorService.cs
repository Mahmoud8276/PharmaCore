using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.IServices;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.Services
{
    public class PackagingConvertorService : IPackagingConvertorService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PackagingConvertorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<int> ToBaseUnit(int productId, int PackagingUnitId, decimal quantity)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<int> ToBaseUnit(int PackagingLevelId, decimal quantity)
        {
            throw new global::System.NotImplementedException();
        }

        public Task<int> ToBaseUnit(PackagingLevelSnapshot packagingSnapshot, decimal quantity)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
