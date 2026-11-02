using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
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


        public Task GetAllAsync(InventorySpecParams spec)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
