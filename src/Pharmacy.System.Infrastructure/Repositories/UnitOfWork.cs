using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;
using Pharmacy.System.Core.Interfaces.IRepositories;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<IProductBarcodeRepository> _productBarcodeRepository;
        private readonly Lazy<IProductCategoryRepository> _productCategoryRepository;
        private readonly Lazy<IManufactorerRepository> _manufacturerRepository;
        private readonly Lazy<IProductFormRepository> _productFormRepository;
        private readonly Lazy<IBaseUnitRepository> _baseUnitRepository;
        private readonly Lazy<IActiveIngredientRepository> _activeIngredientRepository;
        private readonly Lazy<IProductActiveIngredientRepository> _productActiveIngredientRepository;
        private readonly Lazy<IProductPackagingLevelRepository> _productPackagingLevelRepository;
        private readonly Lazy<IPackagingUnitRepository> _packagingUnitRepository;
        private readonly Lazy<ISupplierRepository> _supplierRepository;
        private readonly Lazy<ISupplierCreditRepository> _supplierCreditRepository;
        private readonly Lazy<ISupplyRepository> _supplyRepository;
        private readonly Lazy<ISupplyPaymentRepository> _supplyPaymentRepository;
        private readonly Lazy<ISupplyInstallmentRepository> _supplyInstallmentRepository;
        private readonly Lazy<IRepositoryRepository> _repositoryRepository;
        private readonly Lazy<IInventoryItemRepository> _inventoryItemRepository;


        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(_context));
            _productBarcodeRepository = new Lazy<IProductBarcodeRepository>(() => new ProductBarcodeRepository(_context));
            _productCategoryRepository = new Lazy<IProductCategoryRepository>(() => new ProductCategoryRepository(_context));
            _manufacturerRepository = new Lazy<IManufactorerRepository>(() => new ManufactorerRepository(_context));
            _productFormRepository = new Lazy<IProductFormRepository>(() => new ProductFormRepository(_context));
            _baseUnitRepository = new Lazy<IBaseUnitRepository>(() => new BaseUnitRepository(_context));
            _activeIngredientRepository = new Lazy<IActiveIngredientRepository>(() => new ActiveIngredientRepository(_context));
            _productActiveIngredientRepository = new Lazy<IProductActiveIngredientRepository>(() => new ProductActiveIngredientRepository(_context));
            _productPackagingLevelRepository = new Lazy<IProductPackagingLevelRepository>(() => new ProductPackagingLevelRepository(_context));
            _packagingUnitRepository = new Lazy<IPackagingUnitRepository>(() => new PackagingUnitRepository(_context));
            _supplierRepository = new Lazy<ISupplierRepository>(() => new SupplierRepository(_context));
            _supplierCreditRepository = new Lazy<ISupplierCreditRepository>(() => new SupplierCreditRepository(_context));
            _supplyRepository = new Lazy<ISupplyRepository>(() => new SupplyRepository(_context));
            _supplyPaymentRepository = new Lazy<ISupplyPaymentRepository>(() => new SupplyPaymentRepository(_context));
            _supplyInstallmentRepository = new Lazy<ISupplyInstallmentRepository>(() => new SupplyInstallmentRepository(_context));
            _repositoryRepository = new Lazy<IRepositoryRepository>(() => new RepositoryRepository(_context));
            _inventoryItemRepository = new Lazy<IInventoryItemRepository>(() => new InventoryItemRepository(_context));
        }

        public IProductRepository ProductRepository => _productRepository.Value;
        public IProductBarcodeRepository ProductBarcodeRepository => _productBarcodeRepository.Value;
        public IProductCategoryRepository ProductCategoryRepository => _productCategoryRepository.Value;
        public IManufactorerRepository ManufacturerRepository => _manufacturerRepository.Value;
        public IProductFormRepository ProductFormRepository => _productFormRepository.Value;
        public IBaseUnitRepository BaseUnitRepository => _baseUnitRepository.Value;
        public IActiveIngredientRepository ActiveIngredientRepository => _activeIngredientRepository.Value;
        public IProductActiveIngredientRepository ProductActiveIngredientRepository => _productActiveIngredientRepository.Value;
        public IProductPackagingLevelRepository ProductPackagingLevelRepository => _productPackagingLevelRepository.Value;
        public IPackagingUnitRepository PackagingUnitRepository => _packagingUnitRepository.Value;
        public ISupplierRepository SupplierRepository => _supplierRepository.Value;
        public ISupplierCreditRepository SupplierCreditRepository => _supplierCreditRepository.Value;
        public ISupplyRepository SupplyRepository => _supplyRepository.Value;
        public ISupplyPaymentRepository SupplyPaymentRepository => _supplyPaymentRepository.Value;
        public ISupplyInstallmentRepository SupplyInstallmentRepository => _supplyInstallmentRepository.Value;
        public IRepositoryRepository RepositoryRepository => _repositoryRepository.Value;
        public IInventoryItemRepository InventoryItemRepository => _inventoryItemRepository.Value;



        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}
