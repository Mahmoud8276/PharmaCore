using Pharmacy.System.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.System.Core.Interfaces.IRepositories
{
    public interface ISupplyPaymentRepository : IGenericRepository<SupplyPayment, int>
    {
        public Task<IReadOnlyList<SupplyPayment>> GetAllBySupplyId(int SupplyId);
    }
}
