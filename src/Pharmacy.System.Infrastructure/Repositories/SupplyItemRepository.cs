using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class SupplyItemRepository : GenericRepository<SupplyItem, int>, ISupplyItemRepository
    {
        public SupplyItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
