using Pharmacy.System.Core.Dtos.SupplyDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using Pharmacy.System.Services.Specifications;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using System.Collections.Generic;
using Pharmacy.System.Core.Dtos;
using System;
using Pharmacy.System.Core.Enums;
using Pharmacy.System.Core.Dtos.SupplyPaymentDtos;

namespace Pharmacy.System.Services.Services
{
    public class SupplyService : ISupplyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SupplyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private sealed record ValidatedSupply(
            Supplier supplier,
            Repository repository,
            Dictionary<int, Product> Products,
            Dictionary<int, ProductPackagingLevel> Levels);

        private async Task<(ValidatedSupply? Data, Response? Error)> ValidateAsync(SupplyDto dto)
        {
            // Check that the receivement date is not in the future.
            if (dto.ReceivedAt > DateOnly.FromDateTime(DateTime.Today))
            {
                return new(null, Response.Fail(
                     message: "Invalid receivement date!"));
            }
            // Check the existance of the supplier.
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
            {
                return new (null, Response.NotFound(
                    message: "Supplier not found!"));
            }

            //Check the existance of the repository
            var repository = await _unitOfWork.RepositoryRepository.GetByIdAsync(dto.RepositoryId);
            if (repository == null)
            {
                return new (null, Response.NotFound(
                    message: "Repository not found!"));
            }

            var productIds = dto.Items.Select(x => x.ProductId).ToHashSet();
            var products = await _unitOfWork.ProductRepository.
                GetAllWithSpecAsync(new ProductSpecification(productIds));

            var packagingLevelIds = dto.Items.Select(x => x.PackagingLevelId).ToHashSet();
            var packagingLevels = await _unitOfWork.ProductPackagingLevelRepository.
                GetAllWithSpecAsync(new ProductPackagingLevelSpecification(packagingLevelIds));

            // For each supply item, check: 
            // The existance of the associated product.
            if (productIds.Count != products.Count)
            {
                var notFoundProductIds = productIds.Except(products.Select(x => x.Id)).ToList();

                return new(null, Response.NotFound(
                    message: "One or more products not found!",
                    details: "Not found product IDs: " + string.Join(", ", notFoundProductIds)));
            }

            // The existance of the Packaging Level of the associated product.
            if (packagingLevelIds.Count != packagingLevels.Count)
            {
                var notFoundPackagingLevelIds = packagingLevelIds.Except(packagingLevels.Select(x => x.Id)).ToList();
                return new(null, Response.NotFound(
                    message: "One or more packaging levels not found!",
                    details: "Not found packaging level IDs: " + string.Join(", ", notFoundPackagingLevelIds)));
            }

            // That this packaging level is for the targeted product.
            var levelsById = packagingLevels.ToDictionary(l => l.Id);

            var mismatched = dto.Items
                .Where(i => levelsById[i.PackagingLevelId].ProductId != i.ProductId)
                .ToList();

            if (mismatched.Count > 0)
            {
                return new(null, Response.Fail(
                    message: "Invalid packaging level for one or more products!",
                    details: string.Join("; ", mismatched.Select(i =>
                        $"Level {i.PackagingLevelId} does not belong to product {i.ProductId}"))));
            }

            // After validating the incoming DTO, map and make the essintial updates to the existing supply entity.
            var productsLookup = products.ToDictionary(p => p.Id);
            var packagingLevelsLookup = packagingLevels.ToDictionary(pu => pu.Id);

            return new (
                new ValidatedSupply(
                supplier: supplier,
                repository: repository,
                Products: productsLookup,
                Levels: packagingLevelsLookup
            ), null);
        }

        private static void BuildItems(SupplyDto dto, Supply supply, ValidatedSupply data)
        {
            supply.Items.Clear(); // Clear existing items to replace with new ones
            foreach (var item in dto.Items)
            {
                var packagingLevel = data.Levels[item.PackagingLevelId];
                var product = data.Products[item.ProductId];

                var supplyItem = item.Adapt<SupplyItem>();
                supplyItem.Supply = supply;
                supplyItem.BaseUnitsPerReceivedUnit = packagingLevel.QuantityOfBaseUnit;
                supplyItem.TotalBaseUnits = supplyItem.BaseUnitsPerReceivedUnit * item.Quantity;
                supplyItem.UnitName = packagingLevel.PackagingUnit.Name;
                supplyItem.EffectiveUnitCost = item.PricePerUnit * (1 - (dto.DiscountPercentage ?? 0) / 100m);
                supplyItem.Product = product;
                supplyItem.ProductSnapshot = product.Adapt<ProductSnapshot>();
                supplyItem.PackagingLevel = packagingLevel;
                supplyItem.PackagingLevelsSnapshot = product.ProductPackagingLevels.Adapt<List<PackagingLevelSnapshot>>();

                supply.Items.Add(supplyItem);
            }
        }



        public async Task<Response> CreateAsync(SupplyDto dto)
        {
            var result = await ValidateAsync(dto);
            if(result.Error != null)
            {
                return result.Error;
            }
            var supply = dto.Adapt<Supply>();
            supply.Supplier = result.Data.supplier;
            supply.Repository = result.Data.repository;

            BuildItems(dto, supply, result.Data);

            // Add the supply and its items to the database
            await _unitOfWork.SupplyRepository.AddAsync(supply);
            await _unitOfWork.CompleteAsync();

            return Response.Created(
                data: supply.Adapt<SupplyDetailsDto>(),
                message: "Supply created successfully!");
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var supply = await _unitOfWork.SupplyRepository.GetWithSpecAsync(
                new SupplySpecification(id));
            if(supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }

            return Response.Success(
                message: "Supply data retrieved successfully",
                data: supply.Adapt<SupplyDetailsDto>());
        }

        public async Task<Response> GetAllAsync(SupplySpecParams specParams)
        {
            var countSpec = new SupplySpecification(specParams, true);
            var count = await _unitOfWork.SupplyRepository.GetCountWithSpecAsync(countSpec);

            var spec = new SupplySpecification(specParams);
            var data = await _unitOfWork.SupplyRepository.GetAllWithSpecAsync(spec);

            var pagination = new Pagination(
                specParams.PageIndex, 
                specParams.PageSize, 
                count, 
                data.Adapt<List<SupplyDetailsDto>>());

            return Response.Success(
                message: "Supplies data retreived successfully",
                data: pagination);
        }

        public async Task<Response> UpdateAsync(int id, SupplyDto dto)
        {
            var supply = await _unitOfWork.SupplyRepository.GetWithSpecAsync(new SupplySpecification(id));
            // Check the existance of the supply to be updated.
            if (supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }
            // Check if the supply is alread confirmed, if so, it cannot be updated.
            if (supply.IsConfirmed)
            {
                return Response.Fail(
                    message: "Cannot update a confirmed supply!");
            }

            var result = await ValidateAsync(dto);
            if(result.Error != null)
            {
                return result.Error;
            }
            dto.Adapt(supply);
            supply.Supplier = result.Data.supplier;
            supply.Repository = result.Data.repository;
            BuildItems(dto, supply, result.Data); 

            _unitOfWork.SupplyRepository.Update(supply);
            await _unitOfWork.CompleteAsync();


            return Response.Success(
                data: supply.Adapt<SupplyDetailsDto>(),
                message: "Supply updated successfully!");
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var supply = await _unitOfWork.SupplyRepository.GetByIdAsync(id);
            if (supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }
            // Check if the supply is alread confirmed, if so, it cannot be deleted.
            if (supply.IsConfirmed)
            {
                return Response.Fail(
                    message: "Cannot update a confirmed supply!");
            }

            _unitOfWork.SupplyRepository.Delete(supply);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Supply deleted successfully!");
        }

        public async Task<Response> ConfirmAsync(int id, SupplyPaymentConfirmationDto dto)
        {
            var supply = await _unitOfWork.SupplyRepository.GetWithSpecAsync(
                new SupplySpecification(id));

            //Check the existance of the supply
            if(supply == null)
            {
                return Response.NotFound(
                    message: "Supply not found!");
            }

            //Check the supply is not already confirmed
            if (supply.IsConfirmed)
            {
                return Response.Conflict(
                    message: "Supply already confirmed!");
            }

            //Translate the supply items into the corresponding inventory items.
            //(group them by product type, price, and expiry date)
            //(convert supply items quantities into its base units to be stored)
            var inventoryItems = supply.Items.Select(x => x.Adapt<InventoryItem>());
            if(inventoryItems.Any(x=>x.ProductId == -1))
            {
                return Response.NotFound(
                    message: "One or more product are not found!",
                    details: "You may have deleted one or more products that is inside the supply before confirming it!");
            }

            inventoryItems = inventoryItems.GroupBy(x => new
            {
                x.ProductId,
                x.SupplyItemId,
                x.RepositoryId,
                x.ExpiryDate,
                x.PricePerBaseUnits

            }).Select(g => new InventoryItem
            {
                ProductId = g.Key.ProductId,
                SupplyItemId = g.Key.SupplyItemId,
                RepositoryId = g.Key.RepositoryId,
                ExpiryDate = g.Key.ExpiryDate,
                PricePerBaseUnits = g.Key.PricePerBaseUnits,

                ReceivedBaseUnits = g.Sum(x=>x.ReceivedBaseUnits),
                OnHandBaseUnits = g.Sum(x=>x.OnHandBaseUnits),
            
            }).ToList();
            //Store the inventory items into the corresponding repository.
            await _unitOfWork.InventoryItemRepository.AddRangeAsync(inventoryItems);

            //Issue supply payment, or payments based on supply payment plan.
            if(supply.PaymentPlan == SupplyPaymentPlan.FullPayment)
            {
                await _unitOfWork.SupplyPaymentRepository.AddAsync(new SupplyPayment()
                {
                    Amount = supply.FinalCost,
                    CreatedAt = supply.CreatedAt,
                    Method = dto.PaymentMethod,
                    Notes = dto.Notes,
                    PaidAt = supply.ReceivedAt,
                    SupplyId = supply.Id,
                    
                });
                supply.PaymentStatus = PaymentStatus.FullyPaid;
            }

            supply.IsConfirmed = true;
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Supply confirmed sucessfully",
                data : supply.Adapt<SupplyDetailsDto>());
        }

    }
}
