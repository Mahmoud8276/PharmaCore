using Mapster;
using Microsoft.Extensions.Logging;
using Pharmacy.System.Core.Dtos;
using Pharmacy.System.Core.Dtos.ProductActiveIngredientDtos;
using Pharmacy.System.Core.Dtos.ProductDtos;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.Helpers;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using Pharmacy.System.Services.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Pharmacy.System.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IUnitOfWork unitofWork,
            ILogger<ProductService> logger)
        {
            _unitOfWork = unitofWork;
            _logger = logger;
        }

        private async Task<Response> ValidateProductDtoAsync(ProductDto dto, int? excludeProductId = null)
        {
            if (await _unitOfWork.ProductRepository.AnyAsync(x =>
                    x.Barcode == dto.Barcode && (excludeProductId == null || x.Id != excludeProductId)))
            {
                return Response.Conflict
                    (message: "Product barcode already exists!");
            }

            if (dto.ManufacturerId != null)
            {
                if (!await _unitOfWork.ManufacturerRepository.AnyAsync(x => x.Id == dto.ManufacturerId))
                {
                    return Response.NotFound(
                        message: "Manufacturer not found!");
                }
            }

            if (!await _unitOfWork.ProductCategoryRepository.AnyAsync(x => x.Id == dto.CategoryId))
            {
                return Response.NotFound(
                    message: "Product category not found!");
            }

            if (!await _unitOfWork.ProductFormRepository.AnyAsync(x => x.Id == dto.ProductFormId))
            {
                return Response.NotFound(
                    message: "Product form not found!");
            }

            return Response.Success();
        }
        private async Task TryDeleteImageAsync(string imageName)
        {
            try
            {
                await FileHelper.DeleteFileAsync(imageName, "ProductImages");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete image {ImageName}", imageName);
            }
        }


        public async Task<Response> CreateAsync(ProductDto dto)
        {
            var validationResult = await ValidateProductDtoAsync(dto);
            if(validationResult.IsSuccess == false)
            {
                return validationResult;
            }

            var product = dto.Adapt<Product>();

            if(dto.Image != null)
            {
                var imageName = await FileHelper.UploadFileAsync(dto.Image, "ProductImages");
                product.Image = imageName;
            }

            try
            {
                await _unitOfWork.ProductRepository.AddAsync(product);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                try
                {
                    if(!string.IsNullOrEmpty(product.Image))
                    {
                        await FileHelper.DeleteFileAsync(product.Image, "ProductImages");
                    }
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Failed to clean up orphaned image {ImageName} after a failed product creation", product.Image);
                }

                throw;
            }

            var createdProduct = await _unitOfWork.ProductRepository
                .GetWithSpecAsync(new ProductSpecification(product.Id));

            return Response.Created(
            data: createdProduct.Adapt<ProductDetailsDto>(),
            message: "Product created successfully");
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if(product == null)
            {
                return Response.NotFound(
                    message: "Product not found!");
            }

            _unitOfWork.ProductRepository.Delete(product);
            await _unitOfWork.CompleteAsync();

            if (product.Image != null)
            {
                try
                {
                    await FileHelper.DeleteFileAsync(product.Image, "ProductImages");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete image {ImageName} for product {ProductId}", product.Image, product.Id);
                }
            }

            return Response.Success(
            message: "Product deleted successfully");
        }

        public async Task<Response> GetAllAsync(ProductSpecParams specParams)
        {
            var countSpec = new ProductSpecification(specParams, true);
            var count = await _unitOfWork.ProductRepository.GetCountWithSpecAsync(countSpec);

            var spec = new ProductSpecification(specParams);
            var data = await _unitOfWork.ProductRepository.GetAllWithSpecAsync(spec);

            var pagination = new Pagination(
                specParams.PageIndex,
                specParams.PageSize, 
                count,
                data.Adapt<List<ProductDetailsDto>>());

            return Response.Success(
                data: pagination,
                message: "Products retrieved successfully");
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetWithSpecAsync(new ProductSpecification(id));
            if (product == null)
            {
                return Response.NotFound(
                    message: "Product not found!");
            }

            return Response.Success(
                data: product.Adapt<ProductDetailsDto>(),
                message: "Product retrieved successfully");
        }

        public async Task<Response> UpdateAsync(int id, ProductDto dto)
        {
            var product = await _unitOfWork.ProductRepository.GetWithSpecAsync(new ProductSpecification(id));
            if (product == null)
                return Response.NotFound(message: "Product not found!");

            var validationResult = await ValidateProductDtoAsync(dto, excludeProductId: id);
            if (!validationResult.IsSuccess)
                return validationResult;

            var oldImage = product.Image;
            string? newImage = null;

            dto.Adapt(product);

            if (dto.Image != null)
            {
                newImage = await FileHelper.UploadFileAsync(dto.Image, "ProductImages");
                product.Image = newImage;
            }

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                if (newImage != null)
                    await TryDeleteImageAsync(newImage); 
                throw;
            }

            if (newImage != null && !string.IsNullOrEmpty(oldImage))
                await TryDeleteImageAsync(oldImage);       

            return Response.Success(
                data: product.Adapt<ProductDetailsDto>(),
                message: "Product updated successfully");
        }




        public async Task<Response> AddProductActiveIngredientAsync(int productId, int activeIngredientId, ProductActiveIngredientDto dto)
        {
            if(!await _unitOfWork.ProductRepository.AnyAsync(x=>x.Id == productId))
            {
                return Response.NotFound(
                    message: "Product Not Fount");
            }

            var activeIngredient = await _unitOfWork.ActiveIngredientRepository.GetByIdAsync(activeIngredientId);
            if(activeIngredient == null)
            {
                return Response.NotFound(
                    message: "Active Ingredient Not Fount");
            }

            if (await _unitOfWork.ProductActiveIngredientRepository.
                AnyAsync(x=>x.ActiveIngredientId == activeIngredientId && x.ProductId == productId))
            {
                return Response.Conflict(
                    message: "Active Ingredient Exists For This Product!");
            }

            await _unitOfWork.ProductActiveIngredientRepository.AddAsync(new ProductActiveIngredient()
            {
                ProductId = productId,
                ActiveIngredient = activeIngredient,
                Quantity = dto.Quantity,
                Unit = dto.Unit
            });
            await _unitOfWork.CompleteAsync();


            return Response.Created(
                message: $"Active Ingredient Added For The Product Successfully",
                data: activeIngredient.Adapt<ProductActiveIngredientDetailsDto>());
        }

        public async Task<Response> AddProductActiveIngredientsAsync(int productId, List<ProductActiveIngredientAssociationDto> dtos)
        {
            //Check the existence of the product
            if (!await _unitOfWork.ProductRepository.AnyAsync(x=>x.Id == productId))
            {
                return Response.NotFound(
                    message: "Product Not Fount");
            }

            //Check the douplication of the active Ingredients in the incoming request
            var incomingActiveIngredientsIds = dtos.Select(x => x.ActiveIngredientId).ToList();
            if(incomingActiveIngredientsIds.Distinct().Count() != incomingActiveIngredientsIds.Count())
            {
                return Response.Fail(message: "Duplicate active ingredient in request!");
            }

            //Check the existence of the active Ingredients
            var validActiveIngredients = await _unitOfWork.ActiveIngredientRepository.
                FindAsync(x => incomingActiveIngredientsIds.Contains(x.Id));
            if (validActiveIngredients.Count != incomingActiveIngredientsIds.Count)
            {
                return Response.NotFound(
                    message: "One or more active ingredients not found!");
            }

            //Check if any of the incoming active ingredients already associated with the product
            if (await _unitOfWork.ProductActiveIngredientRepository
                .AnyAsync(x => incomingActiveIngredientsIds.Contains(x.ActiveIngredientId) && x.ProductId == productId))
            {
                return Response.Conflict(
                    message: "One or more active ingredients already associated with this product!");
            }

            //Associate the active ingredients with the product
            var incomingIngredientDetails = dtos.ToDictionary(x => x.ActiveIngredientId);
            var poductIngredientsToBeAdded = validActiveIngredients.Select(
                x => new ProductActiveIngredient()
                {
                    ActiveIngredient = x,
                    ProductId = productId,
                    Quantity = incomingIngredientDetails[x.Id].Quantity,
                    Unit = incomingIngredientDetails[x.Id].Unit
                }).ToList();
            
            await _unitOfWork.ProductActiveIngredientRepository.AddRangeAsync(poductIngredientsToBeAdded);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Active Ingredients Added Successfully",
                data: poductIngredientsToBeAdded.Adapt<List<ProductActiveIngredientDetailsDto>>());
        }

        public async Task<Response> GetProductActiveIngredientsAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetWithSpecAsync(new ProductSpecification(productId));
            if (product == null)
            {
                return Response.NotFound(
                    message: "Product Not Fount");
            }

            return Response.Success(
                message: "Product Active Ingredients Retreived Successfully",
                data: product.ProductActiveIngredients.Adapt<List<ProductActiveIngredientDetailsDto>>());
        }

        public async Task<Response> UpdateProductActiveIngredientAsync(int productId, int activeIngredientId, ProductActiveIngredientDto dto)
        {
            var product = await _unitOfWork.ProductRepository.
                GetWithSpecAsync(new ProductSpecification(productId));
            if (product == null)
            {
                return Response.NotFound(
                    message: "Product Not Fount");
            }

            if(!await _unitOfWork.ActiveIngredientRepository.AnyAsync(x=>x.Id == activeIngredientId))
            {
                return Response.NotFound(
                     message: "Active Ingredient Not Fount");
            }

            var association = await _unitOfWork.ProductActiveIngredientRepository
                .GetWithSpecAsync(new ProductActiveIngredientSpecification(productId, activeIngredientId));
            if(association == null)
            {
                return Response.NotFound(
                    message: "Active Ingredient Not Associated With This Product");
            }

            dto.Adapt(association);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Product Active Ingredient Updated Successfully",
                data: association.Adapt<ProductActiveIngredientDetailsDto>());
        }

        public async Task<Response> UpdateProductActiveIngredientsAsync(int productId, List<ProductActiveIngredientAssociationDto> dtos)
        {
            //Check the existence of the product
            if (!await _unitOfWork.ProductRepository.AnyAsync(x => x.Id == productId))
            {
                return Response.NotFound(
                    message: "Product Not Fount");
            }

            //Check the douplication of the active Ingredients in the incoming request
            var incomingActiveIngredientsIds = dtos.Select(x => x.ActiveIngredientId).ToList();
            if (incomingActiveIngredientsIds.Distinct().Count() != incomingActiveIngredientsIds.Count())
            {
                return Response.Fail(message: "Duplicate active ingredient in request!");
            }

            //Check the existence of the active Ingredients
            var validActiveIngredients = await _unitOfWork.ActiveIngredientRepository.
                FindAsync(x => incomingActiveIngredientsIds.Contains(x.Id));
            if (validActiveIngredients.Count != incomingActiveIngredientsIds.Count)
            {
                return Response.NotFound(
                    message: "One or more active ingredients not found!");
            }

            //1.Get products active ingredients
            var existingAssociations = await _unitOfWork.ProductActiveIngredientRepository
                .FindAsync(x => x.ProductId == productId);

            //2.Get the associations that must be added
            var associationsToBeAdded = validActiveIngredients
                .Where(x => !existingAssociations.Any(e => e.ActiveIngredientId == x.Id))
                .Select(x => new ProductActiveIngredient()
                {
                    ProductId = productId,
                    ActiveIngredient = x,
                    Quantity = dtos.First(d => d.ActiveIngredientId == x.Id).Quantity,
                    Unit = dtos.First(d => d.ActiveIngredientId == x.Id).Unit
                }).ToList();

            if(associationsToBeAdded.Count > 0)
            {
                await _unitOfWork.ProductActiveIngredientRepository.AddRangeAsync(associationsToBeAdded);
            }

            //3.Get the associations that must be updated
            var associationsToBeUpdated = existingAssociations
                .Where(x => incomingActiveIngredientsIds.Contains(x.ActiveIngredientId))
                .ToList();

            if(associationsToBeUpdated.Count > 0)
            {
                foreach (var association in associationsToBeUpdated)
                {
                    var incomingDto = dtos.First(d => d.ActiveIngredientId == association.ActiveIngredientId);
                    association.Quantity = incomingDto.Quantity;
                    association.Unit = incomingDto.Unit;
                }
            }

            //4.Get the associations that must be removed
            var associationsToBeRemoved = existingAssociations
                .Where(x => !incomingActiveIngredientsIds.Contains(x.ActiveIngredientId))
                .ToList();

            if (associationsToBeRemoved.Count > 0)
            {
                _unitOfWork.ProductActiveIngredientRepository.DeleteRange(associationsToBeRemoved);
            }

            await _unitOfWork.CompleteAsync();

            var finalAssociations = existingAssociations
                .Except(associationsToBeRemoved)
                .Concat(associationsToBeAdded)
                .ToList();

            return Response.Success(
                message: "Product Active Ingredients Updated Successfully",
                data: finalAssociations.Adapt<List<ProductActiveIngredientDetailsDto>>());
        }

        public async Task<Response> DeleteProductActiveIngredientAsync(int productId, int activeIngredientId)
        {
            if(!await _unitOfWork.ProductRepository.AnyAsync(x => x.Id == productId))
            {
                return Response.NotFound(
                    message: "Product Not Fount");
            }

            if(!await _unitOfWork.ActiveIngredientRepository.AnyAsync(x => x.Id == activeIngredientId))
            {
                return Response.NotFound(
                    message: "Active Ingredient Not Fount");
            }

            var association = await _unitOfWork.ProductActiveIngredientRepository
                .GetWithSpecAsync(new ProductActiveIngredientSpecification(productId, activeIngredientId));
            if (association == null)
            {
                return Response.NotFound(
                    message: "Active Ingredient Not Associated With This Product");
            }

            _unitOfWork.ProductActiveIngredientRepository.Delete(association);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "Product Active Ingredient Deleted Successfully");
        }

        public async Task<Response> DeleteProductActiveIngredientsAsync(int productId)
        {
            if(!await _unitOfWork.ProductRepository.AnyAsync(x => x.Id == productId))
            {
                return Response.NotFound(
                    message: "Product Not Fount");
            }

            var associations = await _unitOfWork.ProductActiveIngredientRepository
                .FindAsync(x => x.ProductId == productId);
            if (associations.Count == 0)
            {
                return Response.NotFound(
                    message: "No Active Ingredients Associated With This Product");
            }

            _unitOfWork.ProductActiveIngredientRepository.DeleteRange(associations);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                message: "All Product Active Ingredients Deleted Successfully");
        }



        
    }
}
