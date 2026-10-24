using Microsoft.EntityFrameworkCore.Storage;
using Pharmacy.System.Core.Interfaces.IRepositories;
using System;
using System.Threading.Tasks;

namespace Pharmacy.System.Core.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IProductRepository ProductRepository { get; }
        public IProductBarcodeRepository ProductBarcodeRepository { get; }
        public IProductCategoryRepository ProductCategoryRepository { get; }
        public IManufactorerRepository ManufacturerRepository { get; }
        public IProductFormRepository ProductFormRepository { get; }
        public IBaseUnitRepository BaseUnitRepository { get; }
        public IActiveIngredientRepository ActiveIngredientRepository { get; }
        public IProductActiveIngredientRepository ProductActiveIngredientRepository { get; }
        public IProductPackagingLevelRepository ProductPackagingLevelRepository { get; }
        public IPackagingUnitRepository PackagingUnitRepository { get; }
        public ISupplierRepository SupplierRepository { get; }
        public ISupplyRepository SupplyRepository { get; }
        public ISupplyPaymentRepository SupplyPaymentRepository { get; }
        public ISupplyInstallmentRepository SupplyInstallmentRepository { get; }
        public IRepositoryRepository RepositoryRepository { get; }
        public IInventoryItemRepository InventoryItemRepository { get; }

        public Task CompleteAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public Task RollbackTransactionAsync();
    }
}
