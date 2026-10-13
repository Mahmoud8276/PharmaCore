using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class SupplyRepository : GenericRepository<Supply, int>, ISupplyRepository
    {
        private readonly AppDbContext _context;
        public SupplyRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
