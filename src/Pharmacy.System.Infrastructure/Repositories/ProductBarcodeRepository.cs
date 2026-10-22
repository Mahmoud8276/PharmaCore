using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class ProductBarcodeRepository : GenericRepository<ProductBarcode, int>, IProductBarcodeRepository
    {
        public ProductBarcodeRepository(AppDbContext context) : base(context)
        {
        }
    }
}
