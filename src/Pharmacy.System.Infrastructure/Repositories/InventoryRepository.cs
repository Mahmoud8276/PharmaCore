using Mapster;
using Microsoft.EntityFrameworkCore;
using Pharmacy.System.Core.Dtos;
using Pharmacy.System.Core.Dtos.InventoryDtos;
using Pharmacy.System.Core.Dtos.ProductDtos;
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
            IEnumerable<InventoryItem> inventoryItems)
        {
            var quantities = new Dictionary<string, decimal>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in inventoryItems)
            {
                var remainingUnits = item.OnHandBaseUnits;

                var packagingLevels = item.SupplyItem.PackagingLevelsSnapshot?
                    .Where(x =>
                        x.QuantityOfBaseUnit > 0 &&
                        !string.IsNullOrWhiteSpace(x.UnitName))
                    .OrderByDescending(x => x.QuantityOfBaseUnit)
                    .ToList() ?? new();

                foreach (var level in packagingLevels)
                {
                    var quantity = decimal.Floor(
                        remainingUnits / level.QuantityOfBaseUnit);

                    if (quantity <= 0)
                        continue;

                    var unitName = level.UnitName.Trim();

                    if (!quantities.ContainsKey(unitName))
                        quantities[unitName] = 0;

                    quantities[unitName] += quantity;

                    remainingUnits -= quantity * level.QuantityOfBaseUnit;
                }

                // Remaining base units
                var baseUnitName =
                    item.Product.ProductForm.BaseUnit.Name;

                if (remainingUnits > 0)
                {
                    if (string.IsNullOrWhiteSpace(baseUnitName))
                        baseUnitName = "Base units";

                    baseUnitName = baseUnitName.Trim();

                    if (!quantities.ContainsKey(baseUnitName))
                        quantities[baseUnitName] = 0;

                    quantities[baseUnitName] += remainingUnits;
                }
            }

            return string.Join(
                " + ",
                quantities
                    .Where(x => x.Value > 0)
                    .Select(x =>
                        $"{x.Value.ToString("0.####", CultureInfo.InvariantCulture)} {x.Key}"));
        }


        public async Task<Pagination> GetAllInventoryGroupedByProduct(IInventorySpecParams specParams)
        {
            var items = _context.InventoryItems.AsNoTracking()
                .Where(x => x.OnHandBaseUnits > 0);

            // Filter using spacification parameters
            if (specParams.RepositoryId.HasValue)
                items.Where(x => x.RepositoryId == specParams.RepositoryId);

            if(specParams.CategoryId.HasValue)
                items.Where(x=>x.Product.CategoryId == specParams.CategoryId);
            
            if(!string.IsNullOrEmpty(specParams.Name))
            {
                var name = specParams.Name.Trim().ToLower();
                items.Where(x => x.Product.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(specParams.Barcode))
            {
                var barcode = specParams.Barcode.Trim();
                items.Where(x => x.Product.Barcodes.Any(x => x.Barcode == barcode));
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
            var inventoryItems = await items.Where(x => productIds.Contains(x.Id))
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
                        product.MinimumStockLevel.HasValue &&
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
    }
}
