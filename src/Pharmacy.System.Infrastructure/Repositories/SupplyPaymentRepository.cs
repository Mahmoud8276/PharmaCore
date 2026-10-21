using Microsoft.EntityFrameworkCore;
using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class SupplyPaymentRepository : GenericRepository<SupplyPayment, int>, ISupplyPaymentRepository
    {
        private readonly AppDbContext _context;
        public SupplyPaymentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<SupplyPayment>> GetAllBySupplyId(int SupplyId)
        {
            return await _context.SupplyPayments
                .Where(sp => sp.SupplyId == SupplyId)
                .ToListAsync();
        }
    }
}
