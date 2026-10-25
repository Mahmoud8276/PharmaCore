using Pharmacy.System.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.System.Core.Interfaces.IRepositories
{
    public interface ISupplyInstallmentRepository : IGenericRepository<SupplyInstallment, int>
    {
        public Task<List<SupplyInstallment>> GetSupplyInstallmentsAsync(int supplyId);
    }
}
