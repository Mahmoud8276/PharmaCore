using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class ProductBatchRepository : GenericRepository<ProductBatch, int>, IProductBatchRepository
    {
        public ProductBatchRepository(AppDbContext context) : base(context)
        {
        }
    }
}
