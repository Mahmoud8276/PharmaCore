using Microsoft.EntityFrameworkCore;
using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class SupplyInstallmentRepository : GenericRepository<SupplyInstallment, int>, ISupplyInstallmentRepository
    {
        public SupplyInstallmentRepository(AppDbContext context) : base(context) { }

        public async Task<List<SupplyInstallment>> GetSupplyInstallments(int supplyId)
        {
            return await _context.SupplyInstallments
                .Include(x=>x.Payments)
                .Where(x=>x.SupplyId == supplyId)
                .ToListAsync();
        }
    }
}
