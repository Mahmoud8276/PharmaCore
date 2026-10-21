using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class InventoryItemRepository : GenericRepository<InventoryItem, int>, IInventoryItemRepository
    {
        public InventoryItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
