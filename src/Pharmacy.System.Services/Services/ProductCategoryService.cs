using Mapster;
using Microsoft.Extensions.Logging;
using Pharmacy.System.Core.Dtos;
using Pharmacy.System.Core.Dtos.ProductCategoryDtos;
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

namespace Pharmacy.System.Services.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductCategoryService> _logger;

        public ProductCategoryService(
            IUnitOfWork unitOfWork,
            ILogger<ProductCategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<Response> CreateAsync(ProductCategoryDto dto)
        {
            var productCategory = dto.Adapt<ProductCategory>();
            productCategory.Image = null;

            if (dto.Image != null)
            {
                try
                {
                    productCategory.Image = await FileHelper.UploadFileAsync(dto.Image, "CategoryImages");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to upload image file for the created category.");
                    return Response.Fail(
                        message: "Failed to upload category image!");
                }
            }


            try 
            { 
                await _unitOfWork.ProductCategoryRepository.AddAsync(productCategory);
                await _unitOfWork.CompleteAsync();
            }
            catch
            {
                if(productCategory.Image != null)
                {
                    try
                    {
                        await FileHelper.DeleteFileAsync(productCategory.Image, "CategoryImages");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to upload image file for the created category.");
                    }
                }
                throw;
            }

            return Response.Created(
                data: productCategory.Adapt<ProductCategoryDetailsDto>(),
                message: "Product Category Created Successfully!");
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var productCategory = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(id);
            if (productCategory == null)
                return Response.NotFound(
                    message: "Product category not found!");

            if (await _unitOfWork.ProductRepository.AnyAsync(x => x.CategoryId == id))
            {
                return Response.Conflict(
                    message: "Could not delete this product category", 
                    details: $"{productCategory.Name} product category has one or more products associated with it!");
            }

            _unitOfWork.ProductCategoryRepository.Delete(productCategory);
            await _unitOfWork.CompleteAsync();

            return Response.Success(message: $"{productCategory.Name} product category deleted successfully");
        }

        public async Task<Response> GetAllAsync(ProductCategorySpecParams specParams)
        {
            var countSpec = new ProductCategorySpecification(specParams, true);
            var count = await _unitOfWork.ProductCategoryRepository.GetCountWithSpecAsync(countSpec);

            var spec = new ProductCategorySpecification(specParams);
            var productCategories = await _unitOfWork.ProductCategoryRepository.GetAllWithSpecAsync(spec);

            var pagination = new Pagination(
                specParams.PageIndex,
                specParams.PageSize,
                count,
                productCategories.Adapt<List<ProductCategoryDetailsDto>>());

            return Response.Success(
                data: pagination,
                message: "Product Categories retrieved successfully");
        }

        public async Task<Response> GetByIdAsync(int id)
        {
            var productCategory = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(id);
            if (productCategory == null)
                return Response.NotFound(
                    message: "Product category not found!");

            return Response.Success(
                data: productCategory.Adapt<ProductCategoryDetailsDto>(),
                message: "Product Category retrieved successfully");
        }

        public async Task<Response> UpdateAsync(UpdateProductCategoryDto dto, int id)
        {
            var productCategory = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(id);
            if (productCategory == null)
                return Response.NotFound(
                    message: "Product category not found!");

            dto.Adapt(productCategory);

            _unitOfWork.ProductCategoryRepository.Update(productCategory);
            await _unitOfWork.CompleteAsync();

            return Response.Success(
                data: productCategory.Adapt<ProductCategoryDetailsDto>(),
                message: "Product Category updated successfully");
        }

        public async Task<Response> UpdateCategoryImageAsync(int id, UpdateCategoryImageDto dto)
        {
            var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(id);
            if (category == null)
                return Response.NotFound(
                    message: "Category not found!");

            var oldImage = category.Image;
            string newImage;
            try
            {
                newImage = await FileHelper.UploadFileAsync(dto.Image, "CategoryImages");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't upload the image for category {CategoryId}.", id);
                return Response.Fail(
                    message: "Failed to upload the category image!");
            }

            category.Image = newImage;

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch
            {
                try { await FileHelper.DeleteFileAsync(newImage, "CategoryImages"); }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Couldn't remove the orphaned image {Image} for category {CategoryId}.",
                        newImage, id);
                }
                throw;
            }

            if (!string.IsNullOrEmpty(oldImage))
            {
                try { await FileHelper.DeleteFileAsync(oldImage, "CategoryImages"); }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Couldn't delete the old image {Image} for category {CategoryId}.",
                        oldImage, id);
                }
            }

            return Response.Success(
                message: "Category image updated successfully",
                data: category.Adapt<ProductCategoryDetailsDto>());
        }

        public async Task<Response> DeleteCategoryImageAsync(int id)
        {
            var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(id);
            if (category == null)
                return Response.NotFound(
                    message: "Category not found!");

            if (category.Image == null)
                return Response.NotFound(
                    message: "The category has no image!");

            var oldImage = category.Image;

            category.Image = null;
            await _unitOfWork.CompleteAsync();

            try
            {
                await FileHelper.DeleteFileAsync(oldImage, "CategoryImages");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Couldn't delete the image file {Image} for category {CategoryId}.",
                    oldImage, id);
            }

            return Response.Success(message: "Category image deleted successfully");
        }
    }
}
