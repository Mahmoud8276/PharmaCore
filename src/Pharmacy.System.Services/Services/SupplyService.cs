using Pharmacy.System.Core.Dtos.SupplyDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using Pharmacy.System.Services.Specifications;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mapster;
using System.Collections.Generic;
using Pharmacy.System.Core.Dtos;
using System;

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
            Dictionary<int, Product> Products,
            Dictionary<int, ProductPackagingLevel> Levels);

        private async Task<(ValidatedSupply? Data, Response? Error)> ValidateAsync(SupplyDto dto)
        {
            // Check that the initial paid amount is not greater than the final cost.
            var finalCost = dto.Items.Sum(x => x.Quantity * x.PricePerUnit)
                        * (1 - (dto.DiscountPercentage ?? 0) / 100m);
            if (dto.InitialPaidAmount > finalCost)
            {
                return new(null, Response.Fail(
                     message: "Initial paid amount cannot be greater than the final cost!",
                     statusCode: (int)HttpStatusCode.BadRequest));
            }

            // Check that the receivement date is not in the future.
            if (dto.ReceivedAt > new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day))
            {
                return new(null, Response.Fail(
                     message: "Invalid receivement date!",
                     statusCode: (int)HttpStatusCode.BadRequest));
            }
            // Check the existance of the supplier.
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
            {
                return new (null, Response.Fail(
                    message: "Supplier not found!",
                    statusCode: (int)HttpStatusCode.NotFound));
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

                return new(null, Response.Fail(
                    message: "One or more products not found!",
                    details: "Not found product IDs: " + string.Join(", ", notFoundProductIds),
                    statusCode: (int)HttpStatusCode.NotFound));
            }

            // The existance of the Packaging Level of the associated product.
            if (packagingLevelIds.Count != packagingLevels.Count)
            {
                var notFoundPackagingLevelIds = packagingLevelIds.Except(packagingLevels.Select(x => x.Id)).ToList();
                return new(null, Response.Fail(
                    message: "One or more packaging levels not found!",
                    details: "Not found packaging level IDs: " + string.Join(", ", notFoundPackagingLevelIds),
                    statusCode: (int)HttpStatusCode.NotFound));
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
                        $"Level {i.PackagingLevelId} does not belong to product {i.ProductId}")),
                    statusCode: (int)HttpStatusCode.BadRequest));
            }

            // After validating the incoming DTO, map and make the essintial updates to the existing supply entity.
            var productsLookup = products.ToDictionary(p => p.Id);
            var packagingLevelsLookup = packagingLevels.ToDictionary(pu => pu.Id);

            return new (
                new ValidatedSupply(
                supplier: supplier,
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

            BuildItems(dto, supply, result.Data);

            // Add the supply and its items to the database
            await _unitOfWork.SupplyRepository.AddAsync(supply);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                data: supply.Adapt<SupplyDetailsDto>(),
                message: "Supply created successfully!",
                statusCode: (int)HttpStatusCode.Created);
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var supply = await _unitOfWork.SupplyRepository.GetWithSpecAsync(
                new SupplySpecification(id));
            if(supply == null)
            {
                return Response.Fail(
                    message: "Supply not found!",
                    statusCode: (int)HttpStatusCode.NotFound);
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
                return Response.Fail(
                    message: "Supply not found!",
                    statusCode: (int)HttpStatusCode.NotFound);
            }
            // Check if the supply is alread confirmed, if so, it cannot be updated.
            if (supply.IsConfirmed)
            {
                return Response.Fail(
                    message: "Cannot update a confirmed supply!",
                    statusCode: (int)HttpStatusCode.BadRequest);
            }

            var result = await ValidateAsync(dto);
            if(result.Error != null)
            {
                return result.Error;
            }
            dto.Adapt(supply);
            supply.Supplier = result.Data.supplier;
            BuildItems(dto, supply, result.Data); 

            _unitOfWork.SupplyRepository.Update(supply);
            await _unitOfWork.CompleteAsync();


            return Response.Success(
                data: supply.Adapt<SupplyDetailsDto>(),
                message: "Supply updated successfully!",
                statusCode: (int)HttpStatusCode.OK);
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var supply = await _unitOfWork.SupplyRepository.GetByIdAsync(id);
            if (supply == null)
            {
                return Response.Fail(
                    message: "Supply not found!",
                    statusCode: (int)HttpStatusCode.NotFound);
            }
            // Check if the supply is alread confirmed, if so, it cannot be deleted.
            if (supply.IsConfirmed)
            {
                return Response.Fail(
                    message: "Cannot update a confirmed supply!",
                    statusCode: (int)HttpStatusCode.BadRequest);
            }

            _unitOfWork.SupplyRepository.Delete(supply);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Supply deleted successfully!",
                statusCode: (int)HttpStatusCode.OK);
        }

        public Task<Response> ConfirmAsync(int id)
        {
            throw new global::System.NotImplementedException();
        }

    }
}
