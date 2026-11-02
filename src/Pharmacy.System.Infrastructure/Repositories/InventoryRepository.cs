using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Infrastructure.DbContexts;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }


    }
}
