using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Api.Responses;
using Pharmacy.System.Core.Dtos.ProductActiveIngredientDtos;
using Pharmacy.System.Core.Dtos.ProductDtos;
using Pharmacy.System.Core.Dtos.ProductPackagingDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductPackagingService _productPackagingService;
        public ProductController(
            IProductService productService,
            IProductPackagingService productPackagingService)
        {
            _productService = productService;
            _productPackagingService = productPackagingService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] ProductDto dto)
        {
            var response = await _productService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductSpecParams specParams)
        {
            var response = await _productService.GetAllAsync(specParams);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var response = await _productService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] ProductDto dto)
        {
            var response = await _productService.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _productService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }



        [HttpPost("{productId}/active-ingredients")]
        public async Task<IActionResult> AddProductActiveIngredientsAsync(
            int productId, 
            List<ProductActiveIngredientAssociationDto> dtos)
        {
            if(dtos.Count == 0)
            {
                return BadRequest(ApiResponse.Fail(message: "The list of active ingredients cannot be empty."));
            }

            var result = await _productService.AddProductActiveIngredientsAsync(productId, dtos);
            return StatusCode(result.StatusCode, result);
        }
        
        [HttpPost("{productId}/active-ingredients/{activeIngredientId}")]
        public async Task<IActionResult> AddProductActiveIngredientAsync(
            int productId,
            int activeIngredientId, 
            ProductActiveIngredientDto dto)
        {
            var result = await _productService.AddProductActiveIngredientAsync(productId, activeIngredientId, dto);
            return StatusCode(result.StatusCode, result);
        }



        [HttpGet("{productId}/active-ingredients")]
        public async Task<IActionResult> GetProductActiveIngredientsAsync(int productId)
        {
            var result = await _productService.GetProductActiveIngredientsAsync(productId);
            return StatusCode(result.StatusCode, result);
        }



        [HttpPut("{productId}/active-ingredients")]
        public async Task<IActionResult> UpdateProductActiveIngredientsAsync(
            int productId,
            List<ProductActiveIngredientAssociationDto> dtos)
        {
            if (dtos.Count == 0)
            {
                return BadRequest(ApiResponse.Fail(message: "The list of active ingredients cannot be empty."));
            }

            var result = await _productService.UpdateProductActiveIngredientsAsync(productId, dtos);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{productId}/active-ingredients/{activeIngredientId}")]
        public async Task<IActionResult> UpdateProductActiveIngredientAsync(
            int productId,
            int activeIngredientId,
            ProductActiveIngredientDto dto)
        {
            var result = await _productService.UpdateProductActiveIngredientAsync(productId, activeIngredientId, dto);
            return StatusCode(result.StatusCode, result);
        }



        [HttpDelete("{productId}/active-ingredients")]
        public async Task<IActionResult> DeleteProductActiveIngredientsAsync(int productId)
        {
            var result = await _productService.DeleteProductActiveIngredientsAsync(productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{productId}/active-ingredients/{activeIngredientId}")]
        public async Task<IActionResult> DeleteProductActiveIngredientAsync(int productId, int activeIngredientId)
        {
            var result = await _productService.DeleteProductActiveIngredientAsync(productId, activeIngredientId);
            return StatusCode(result.StatusCode, result);
        }




        [HttpPost("{productId}/packaging")]
        public async Task<IActionResult> AddProductPackagingAsync(int productId, ProductPackagingDto dto)
        {
            var result = await _productPackagingService.CreateAsync(productId, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{productId}/packaging")]
        public async Task<IActionResult> GetAllProductPackagingAsync(int productId)
        {
            var result = await _productPackagingService.GetProductPackagingLevelsAsync(productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{productId}/packaging/{productPackagingId}")]
        public async Task<IActionResult> UpdateProductPackagingAsync(int productId, int productPackagingId, UpdateProductPackagingDto dto)
        {
            var result = await _productPackagingService.UpdateAsync(productId, productPackagingId, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{productId}/packaging")]
        public async Task<IActionResult> DeleteProductPackagingLevelsAsync(int productId)
        {
            var result = await _productPackagingService.DeleteProductPackagingLevelsAsync(productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{productId}/packaging/{productPackagingId}")]
        public async Task<IActionResult> DeleteProductPackagingAsync(int productId, int productPackagingId)
        {
            var result = await _productPackagingService.DeleteAsync(productId, productPackagingId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{productId}/packaging/base-unit")]
        public async Task<IActionResult> GetProductBaseUnitAsync(int productId)
        {
            var result = await _productPackagingService.GetProductBaseUnit(productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{productId}/default-packaging/{productPackagingId}")]
        public async Task<IActionResult> AddProductDefaultPackagingAsync(int productId, int productPackagingId)
        {
            var result = await _productPackagingService.AddProductDefaultPackagingAsync(productId, productPackagingId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{productId}/default-packaging")]
        public async Task<IActionResult> GetProductDefaultPackagingAsync(int productId)
        {
            var result = await _productPackagingService.GetProductDefaultPackagingAsync(productId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{productId}/default-packaging")]
        public async Task<IActionResult> DeleteProductDefaultPackagingAsync(int productId)
        {
            var result = await _productPackagingService.DeleteProductDefaultPackagingAsync(productId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
