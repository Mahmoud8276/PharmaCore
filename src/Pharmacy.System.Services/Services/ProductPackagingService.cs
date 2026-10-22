using Pharmacy.System.Core.Dtos.ProductPackagingDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using System.Threading.Tasks;
using Mapster;
using Pharmacy.System.Core.Dtos.BaseUnitDtos;
using Pharmacy.System.Core.Models;
using System.Linq;
using Pharmacy.System.Services.Specifications;

namespace Pharmacy.System.Services.Services
{
    public class ProductPackagingService : IProductPackagingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductPackagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<ProductPacagingDetailsDto?> MapToProductPackagingDetails(
            ProductPackagingLevel entity)
        {
            if(entity == null) return null;

            var dto = entity.Adapt<ProductPacagingDetailsDto>();

            var childPackagingLevel = await _unitOfWork.ProductPackagingLevelRepository
                .GetChildPackagingLevelAsync(entity.Id);
            if (childPackagingLevel == null)
            {
                dto.ChildPackagingUnitName =
                    (await _unitOfWork.BaseUnitRepository.GetByProductIdAsync(entity.ProductId))?.Name;
                return dto;
            }

            dto.ChildPackagingUnitName = childPackagingLevel?.PackagingUnit.Name;
            return dto;
        }


        public async Task<Response> CreateAsync(int productId, ProductPackagingDto dto)
        {
            //Check the existence of the product
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if(product == null)
            {
                return Response.NotFound(
                    message: "Product Not Found!");
            }

            //Check the existence of the packaging unit
            var packagingUnit = await _unitOfWork.PackagingUnitRepository
                .GetByIdAsync(dto.PackagingUnitId);
            if (packagingUnit == null)
            {
                return Response.NotFound(
                    message: "Packaging Unit Not Found!");
            }

            //Check if there is already a configuration for this packaging unit
            if(await _unitOfWork.ProductPackagingLevelRepository
                .AnyAsync(x=>x.ProductId == productId && x.PackagingUnitId == dto.PackagingUnitId))
            {
                return Response.Conflict(
                    message: "There Is Already a Configuration For this Packaging Unit With this Product!");
            }

            //Check if it is the first time to configure a package unit of this product or not
            if(!await _unitOfWork.ProductPackagingLevelRepository
                .AnyAsync(x => x.ProductId == productId))
            {
                //If its is the first time configuring packaging unit for this product,
                //then the user is configuring the unit that will contain the base unit.

                //Check if the client sends a package unit ID, then it will be a bad request
                if(dto.ChildUnitId != null)
                {
                    return Response.Fail(
                        message: "When Configuring a Packaging Unit For The First Time, Child Unit ID Must Be Null");
                } 

                var productPackaging = new ProductPackagingLevel()
                {
                    PackagingUnit = packagingUnit,
                    Product = product,
                    ParentId = null,
                    QuantityOfChildPackage = dto.Quantity,
                    QuantityOfBaseUnit = dto.Quantity,
                };

                await _unitOfWork.ProductPackagingLevelRepository.AddAsync(productPackaging);
                await _unitOfWork.CompleteAsync();

                return Response.Created(
                    message: "Package Unit Configured For This Product Successfully",
                    data: await MapToProductPackagingDetails(productPackaging));
            }
        
            //Check if the client passes the child packaging ID or not, cuz it's not the first time.
            if(dto.ChildUnitId == null)
            {
                return Response.Fail(
                    message: "Child Packaging Level Id Is Required!");
            }

            // Check that this Child package exists and actually belongs to this product
            var childPackaging = await _unitOfWork.ProductPackagingLevelRepository
                .GetByIdAsync((int)dto.ChildUnitId);
            if(childPackaging == null || childPackaging.ProductId != productId)
            {
                return Response.NotFound(
                    message: "Child Packaging Not Found!");
            }

            if(childPackaging.ParentId != null)
            {
                return Response.Conflict(
                    message: "Child Packaging Already Contained In Another Packaging Level!");
            }

            var packagingLevel = new ProductPackagingLevel()
            {
                Product = product,
                PackagingUnit = packagingUnit,
                ParentId = null,
                QuantityOfChildPackage = dto.Quantity,
                QuantityOfBaseUnit = dto.Quantity * childPackaging.QuantityOfBaseUnit,
            };
            childPackaging.Parent = packagingLevel;

            await _unitOfWork.ProductPackagingLevelRepository.AddAsync(packagingLevel);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Package Unit Configured For This Product Successfully",
                data: await MapToProductPackagingDetails(packagingLevel));
        }

        public async Task<Response> DeleteAsync(int productId, int ProductPackagingId)
        {
            var productPackaging = await _unitOfWork.ProductPackagingLevelRepository
                .GetByIdAsync(ProductPackagingId);
            if(productPackaging == null || productPackaging.ProductId != productId)
            {
                return Response.NotFound(
                    message: "Product's Packaging Not Found!");
            }

            if(productPackaging.ParentId != null)
            {
                return Response.Fail(
                    message: "This Product's Packaging Is Contained Into Another Packaging");
            }
            var child = await _unitOfWork.ProductPackagingLevelRepository
                .GetChildPackagingLevelAsync(productPackaging.Id);
            if (child != null) child.ParentId = null;

            _unitOfWork.ProductPackagingLevelRepository
                .Delete(productPackaging);
            await _unitOfWork.CompleteAsync();


            return Response.Success(
                message: "Product Packaging Deleted Successfully");
        }

        public async Task<Response> DeleteProductPackagingLevelsAsync(int productId)
        {
            var packagingLevels = await _unitOfWork.ProductPackagingLevelRepository
                .FindAsync(x=>x.ProductId == productId);
            if (packagingLevels == null || packagingLevels.Count == 0)
            {
                return Response.NotFound(
                    message: "No Packaging Configured for this product!");
            }

            _unitOfWork.ProductPackagingLevelRepository
                .DeleteRange(packagingLevels);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Product's Packaging Deleted Successfully");
        }

        public async Task<Response> GetProductPackagingLevelsAsync(int productId)
        {
            if(!await _unitOfWork.ProductRepository.AnyAsync(x=>x.Id == productId))
            {
                return Response.NotFound(
                    message: "Product Not Found!");
            }

            var packagingLevels = await _unitOfWork
                .ProductPackagingLevelRepository.GetAllWithSpecAsync(
                new ProductPackagingLevelSpecification(productId, null));
            if(packagingLevels == null || packagingLevels.Count == 0)
            {
                return Response.NotFound(
                    message : "No Packaging Configured for this product!");
            }

            var childLoockup = packagingLevels.ToDictionary(
                x => x.Id,
                x => packagingLevels.FirstOrDefault(y => y.ParentId == x.Id));

            var productPackaging = await Task.WhenAll(
                packagingLevels.Select(async x =>
                {
                    var details = x.Adapt<ProductPacagingDetailsDto>();

                    if (childLoockup[x.Id] == null)
                    {
                        details.ChildPackagingUnitName =
                            (await _unitOfWork.BaseUnitRepository
                                .GetByProductIdAsync(productId))?.Name;
                    }
                    else
                    {
                        var child = childLoockup[x.Id];
                        details.ChildPackagingUnitName =
                            child?.PackagingUnit.Name;
                    }

                    return details;
                }));

            return Response.Success(
                message: "Product Packaging Retrived Successfully",
                data: productPackaging.OrderBy(x=>x.Id));
        }

        public async Task<Response> GetProductBaseUnit(int productId)
        {
            if(!await _unitOfWork.ProductRepository.AnyAsync(
                product=> product.Id == productId))
            {
                return Response.NotFound(
                    message: "Product Not Found!");
            }

            var baseUnit = await _unitOfWork.BaseUnitRepository.GetByProductIdAsync(productId);
            return baseUnit == null ? 
                Response.NotFound(message: "Base Unit Not Found!") :
                Response.Success(data: baseUnit.Adapt<BaseUnitDetailsDto>(), message: "Base unit retrieved successfully");
        }

        public async Task<Response> UpdateAsync(int productId, int productPackagingId, UpdateProductPackagingDto dto)
        {
            var productPackagings = await _unitOfWork.ProductPackagingLevelRepository
                .FindAsync(x => x.ProductId == productId);

           var targetProductPackaging = productPackagings.FirstOrDefault(x => x.Id == productPackagingId);
            if (targetProductPackaging == null)
            {
                return Response.NotFound(
                    message: "Product's Packaging Not Found!");
            }

            targetProductPackaging.QuantityOfBaseUnit =
                (dto.Quantity * targetProductPackaging.QuantityOfBaseUnit) / targetProductPackaging.QuantityOfChildPackage;

            targetProductPackaging.QuantityOfChildPackage = dto.Quantity;

            var packagingLookup = productPackagings.ToDictionary(x => x.Id);

            var child = targetProductPackaging;
            while (child.ParentId.HasValue &&
                   packagingLookup.TryGetValue(child.ParentId.Value, out var parent))
            {
                parent.QuantityOfBaseUnit =
                    parent.QuantityOfChildPackage * child.QuantityOfBaseUnit;

                child = parent;
            }

            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Product packaging updated Successfully!");
        }


        public async Task<Response> AddProductDefaultPackagingAsync(int productId, int packagingLevelId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return Response.NotFound(
                    message: "Product not found!");
            }

            var packaging = await _unitOfWork.ProductPackagingLevelRepository
                .GetWithSpecAsync(new ProductPackagingLevelSpecification(packagingLevelId));
            if (packaging == null)
            {
                return Response.NotFound(
                    message: "Packaging level not found!");
            }

            if (packaging.ProductId != productId)
            {
                return Response.Conflict(
                    message: "Packaging level does not belong to the specified product!");
            }

            product.DefaultPackagingId = packagingLevelId;
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Default packaging added successfully!",
                data: await MapToProductPackagingDetails(packaging));
        }

        public async Task<Response> GetProductDefaultPackagingAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.
                GetWithSpecAsync(new ProductSpecification(productId));
            if (product == null)
            {
                return Response.NotFound(
                    message: "Product not found!");
            }

            if (product.DefaultPackaging == null)
            {
                return Response.NotFound(
                    message: "Default packaging not configured for this product!");
            }

            return Response.Success(
                message: "Default packaging retrieved successfully!",
                data: await MapToProductPackagingDetails(product.DefaultPackaging));
        }

        public async Task<Response> DeleteProductDefaultPackagingAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.
                GetWithSpecAsync(new ProductSpecification(productId));
            if (product == null)
            {
                return Response.NotFound(
                    message: "Product not found!");
            }

            if (product.DefaultPackaging == null)
            {
                return Response.NotFound(
                    message: "Default packaging not configured for this product!");
            }

            product.DefaultPackagingId = null;
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Default packging deleted successfully!");
        }

    }
}
