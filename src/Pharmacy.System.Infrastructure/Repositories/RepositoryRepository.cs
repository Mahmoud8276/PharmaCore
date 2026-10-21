using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class RepositoryRepository : GenericRepository<Repository, int>, IRepositoryRepository
    {
        private readonly AppDbContext _context;
        
        public RepositoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
