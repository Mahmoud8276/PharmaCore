using Microsoft.EntityFrameworkCore;
using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class SupplierCreditRepository : GenericRepository<SupplierCredit, int>, ISupplierCreditRepository
    {
        public SupplierCreditRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<SupplierCredit>> GetBySupplyIdAsync(int supplyId)
        {
            return await _context.SupplierCredits
                .Include(x => x.SupplyReturn)
                .Where(sc => sc.SupplyReturn.SupplyId == supplyId)
                .ToListAsync();
        }
    }
}
