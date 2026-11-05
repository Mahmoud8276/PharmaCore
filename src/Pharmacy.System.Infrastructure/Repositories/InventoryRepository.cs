using Mapster;
using Microsoft.EntityFrameworkCore;
using Pharmacy.System.Core.Dtos;
using Pharmacy.System.Core.Dtos.InventoryDtos;
using Pharmacy.System.Core.Dtos.ProductDtos;
using Pharmacy.System.Core.Dtos.RepositoryDtos;
using Pharmacy.System.Core.Enums;
using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.System.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;
        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        private static string CalculateQuantity(
            IEnumerable<InventoryItem> inventoryItems,
            bool remaining = true,
            string baseUnit = null)
        {
            var quantities = new Dictionary<string, decimal>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in inventoryItems)
            {
                var units = remaining? item.OnHandBaseUnits : item.ReceivedBaseUnits;

                var packagingLevels = item.SupplyItem.PackagingLevelsSnapshot?
                    .Where(x =>
                        x.QuantityOfBaseUnit > 0 &&
                        !string.IsNullOrWhiteSpace(x.UnitName))
                    .OrderByDescending(x => x.QuantityOfBaseUnit)
                    .ToList() ?? new();

                foreach (var level in packagingLevels)
                {
                    var quantity = decimal.Floor(
                        units / level.QuantityOfBaseUnit);

                    if (quantity <= 0)
                        continue;

                    var unitName = level.UnitName.Trim();

                    if (!quantities.ContainsKey(unitName))
                        quantities[unitName] = 0;

                    quantities[unitName] += quantity;

                    units -= quantity * level.QuantityOfBaseUnit;
                }

                // Remaining base units
                var baseUnitName = baseUnit??
                    item.Product.ProductForm.BaseUnit.Name;

                if (units > 0)
                {
                    if (string.IsNullOrWhiteSpace(baseUnitName))
                        baseUnitName = "Base units";

                    baseUnitName = baseUnitName.Trim();

                    if (!quantities.ContainsKey(baseUnitName))
                        quantities[baseUnitName] = 0;

                    quantities[baseUnitName] += units;
                }
            }

            return string.Join(
                " + ",
                quantities
                    .Where(x => x.Value > 0)
                    .Select(x =>
                        $"{x.Value.ToString("0.####", CultureInfo.InvariantCulture)} {x.Key}"));
        }



        public async Task<Pagination> GetAllInventoryDetailsWithSpecAsync(IInventorySpecParams specParams)
        {
            var items = _context.InventoryItems.AsNoTracking()
                .Where(x => x.OnHandBaseUnits > 0);

            // Filter using spacification parameters
            if (specParams.RepositoryId.HasValue)
                items = items.Where(x => x.RepositoryId == specParams.RepositoryId);

            if(specParams.CategoryId.HasValue)
                items = items.Where(x=>x.Product.CategoryId == specParams.CategoryId);
            
            if(!string.IsNullOrEmpty(specParams.Name))
            {
                var name = specParams.Name.Trim().ToLower();
                items = items.Where(x => x.Product.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(specParams.Barcode))
            {
                var barcode = specParams.Barcode.Trim();
                items = items.Where(x => x.Product.Barcodes.Any(b => b.Barcode == barcode));
            }

            // Get the total count of the products inside the inventory
            var totalCount = await items.Select(x => x.ProductId).Distinct().CountAsync();

            // Apply pagination, grouping and based on them get
            // the product IDs that we will be retrieving
            var productIds = await items
                .Select(x => new
                {
                    x.ProductId,
                    ProductName = x.Product.Name
                })
                .Distinct()
                .OrderBy(x => x.ProductName)
                .ThenBy(x => x.ProductId)
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .Select(x => x.ProductId)
                .ToListAsync();

            // If there is no product IDs, return an empty pagination object
            if (!productIds.Any())
                return new Pagination(
                    specParams.PageIndex,
                    specParams.PageSize,
                    totalCount,
                    new List<InventoryRowDetailsDto>());

            // Get the inventory items associated with the retrieved product IDs
            var inventoryItems = await items.Where(x => productIds.Contains(x.ProductId))
                        .Include(x => x.Product)
                        .ThenInclude(p => p.Barcodes)
                    .Include(x => x.Product)
                        .ThenInclude(p => p.Manufacturer)
                    .Include(x => x.Product)
                        .ThenInclude(p => p.Category)
                    .Include(x => x.Product)
                        .ThenInclude(p => p.ProductForm)
                            .ThenInclude(f => f.BaseUnit)
                    .Include(x => x.Product)
                        .ThenInclude(p => p.ProductActiveIngredients)
                            .ThenInclude(a => a.ActiveIngredient)
                    .Include(x => x.SupplyItem)
                    .ToListAsync();

            // Group inventory batches by product
            var groupedItems = inventoryItems
                .GroupBy(x => x.ProductId)
                .Select(group =>
                {
                    var product = group.First().Product;

                    var totalBaseUnits = group.Sum(x => x.OnHandBaseUnits);

                    var quantity = CalculateQuantity(group);

                    var status =
                        !product.MinimumStockLevel.HasValue ? InventoryStatus.Undefiend :
                        totalBaseUnits <= (decimal)product.MinimumStockLevel.Value
                            ? InventoryStatus.Low
                            : InventoryStatus.Normal;

                    return new InventoryRowDetailsDto
                    {
                        Product = product.Adapt<ProductDetailsDto>(),
                        Quantity = quantity,
                        Status = status
                    };
                })
                .ToList();

            return new Pagination(
                specParams.PageIndex,
                specParams.PageSize,
                totalCount,
                groupedItems);
        }

        public async Task<ProductStockDetailsDto> GetProductStockDetailsAsync(Product product)
        {
            // Return default values if the product is null
            if (product == null)
                return new ProductStockDetailsDto()
                {
                    Product = product.Adapt<ProductDetailsDto>(),
                    TotalQuantity = "--"
                };

            // Get all inventory items for the specified product that have on-hand stock
            var items = await _context.InventoryItems.AsNoTracking()
                .Where(x => x.ProductId == product.Id && x.OnHandBaseUnits > 0)
                .Include(x => x.Repository)
                .Include(x => x.SupplyItem)
                    .ThenInclude(s => s.PackagingLevelsSnapshot)
                .ToListAsync();

            // If there are no items, return default values
            if (!items.Any())
                return new ProductStockDetailsDto()
                {
                    Product = product.Adapt<ProductDetailsDto>(),
                    TotalQuantity = "--"
                };

            // Calculate total quantity, and total selling price
            // TODO: Calculate total purchase cost
            var totalQuantity = CalculateQuantity(items, true, product.ProductForm.BaseUnit.Name);
            var totalSellingPrice = items.Sum(x => x.OnHandBaseUnits * x.PricePerBaseUnits);
            var status = !product.MinimumStockLevel.HasValue? InventoryStatus.Undefiend :
                items.Sum(x => x.OnHandBaseUnits) <= (decimal)product.MinimumStockLevel.Value
                    ? InventoryStatus.Low
                    : InventoryStatus.Normal;

            // Calculate quantity per repository
            var quantityPerRepository = items
                .GroupBy(x => x.RepositoryId)
                .Select(group =>
                {
                    var repository = group.First().Repository;
                    return new TotalQuantityPerRepositoryDetailsDto
                    {
                        Repository = repository.Adapt<RepositoryDetailsDto>(),
                        Quantity = CalculateQuantity(group, true, product.ProductForm.BaseUnit.Name)
                    };
                })
                .ToList();

            return new ProductStockDetailsDto()
            {
                Product = product.Adapt<ProductDetailsDto>(),
                TotalQuantity = totalQuantity,
                Status = status,
                QuantityPerRepository = quantityPerRepository,
                TotalPurchaseCost = 0, // TODO: Calculate total purchase cost
                TotalSellingPrice = totalSellingPrice
            };
        }

        public async Task<Pagination> GetProductStockGroupDetailsWithSpecAsync(
            Product product,
            IProductStockGroupSpecParams specParams)
        {
            // Apply the filtering to the inventory items for the specified product
            var query = _context.InventoryItems.AsNoTracking()
                .Include(x => x.ProductBatch)
                .Include(x => x.Product)
                    .ThenInclude(p => p.ProductForm)
                        .ThenInclude(f => f.BaseUnit)
                .Include(p => p.Product)
                    .ThenInclude(p => p.ProductPackagingLevels)
                .Include(x=>x.Product)
                    .ThenInclude(p=>p.Category)
                .Include(x=>x.Product)
                    .ThenInclude(p => p.Barcodes)
                .Include(x => x.Repository)
                .Include(x => x.SupplyItem)
                    .ThenInclude(s => s.Supply)
                        .ThenInclude(s => s.Supplier)
                .Where(x => x.ProductId == product.Id && x.OnHandBaseUnits > 0);

            if(specParams.RepositoryId.HasValue)
                query = query.Where(x => x.RepositoryId == specParams.RepositoryId);

            if(specParams.SupplierId.HasValue)
                query = query.Where(x => x.SupplyItem.Supply.SupplierId== specParams.SupplierId);

            if (!string.IsNullOrEmpty(specParams.BatchNumber))
            {
                var batchNumber = specParams.BatchNumber.Trim().ToLower();
                query = query.Where(x => x.ProductBatch.Number.ToLower().Contains(batchNumber));
            }

            if(specParams.FromExpiryDate.HasValue)
                query = query.Where(x => x.ProductBatch.ExpiryDate >= specParams.FromExpiryDate);

            if (specParams.ToExpiryDate.HasValue)
                query = query.Where(x => x.ProductBatch.ExpiryDate <= specParams.ToExpiryDate);

            // Get the total count of the filtered items
            var totalCount = await query.CountAsync();
            if(totalCount == 0)
                return new Pagination(
                    specParams.PageIndex,
                    specParams.PageSize,
                    totalCount, new());

            // Apply pagination and ordering and get the result
            var items = await query.OrderBy(x => x.ProductBatch.ExpiryDate)
                .ThenBy(x => x.ProductBatch.Number)
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ToListAsync();


            List<ProductStockGroupDetails> result = new();
            foreach(var item in items)
            {
                var dto = item.Adapt<ProductStockGroupDetails>();
                dto.RemainingQuantity = CalculateQuantity(
                    new List<InventoryItem> { item },
                    true,
                    product.ProductForm.BaseUnit.Name);

                dto.ReceivedQuantity = CalculateQuantity(
                    new List<InventoryItem> { item },
                    false,
                    product.ProductForm.BaseUnit.Name);
                result.Add(dto);
            }

            return new Pagination(
                specParams.PageIndex,
                specParams.PageSize,
                totalCount,
                result);
        }
    }
}
