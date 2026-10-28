using Pharmacy.System.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.System.Core.Interfaces.IRepositories
{
    public interface ISupplierCreditRepository : IGenericRepository<SupplierCredit, int>
    {
        public Task<IReadOnlyList<SupplierCredit>> GetBySupplyIdAsync(int supplyId);
    }
}
