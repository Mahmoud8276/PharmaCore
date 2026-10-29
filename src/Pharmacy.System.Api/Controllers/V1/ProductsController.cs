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
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductPackagingService _productPackagingService;
        public ProductsController(
            IProductService productService,
            IProductPackagingService productPackagingService)
        {
            _productService = productService;
            _productPackagingService = productPackagingService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] ProductDto dto)
        {
            return FromResponse(
                await _productService.CreateAsync(dto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductSpecParams specParams)
        {
            return FromResponse(
                await _productService.GetAllAsync(specParams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return FromResponse(
                await _productService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateProductDto dto)
        {
            return FromResponse(
                await _productService.UpdateAsync(id, dto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return FromResponse(
                await _productService.DeleteAsync(id));
        }

        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateProductImageAsync(int id, [FromForm] UpdateProductImageDto dto)
        {
            return FromResponse(
                await _productService.UpdateProductImageAsync(id, dto));
        }

        [HttpDelete("{id}/image")]
        public async Task<IActionResult> DeleteProductImageAsync(int id)
        {
            return FromResponse(
                await _productService.DeleteProductImageAsync(id));
        }

        [HttpGet("generate-barcode")]
        public async Task<IActionResult> GenerateBarcodeAsync()
        {
            return FromResponse(
                await _productService.GenerateBarcodeAsync());
        }



        [HttpPost("{productId}/active-ingredients")]
        public async Task<IActionResult> AddProductActiveIngredientsAsync(
            int productId, 
            List<ProductActiveIngredientAssociationDto> dtos)
        {
            if(dtos.Count == 0)
            {
                return FromResponse(
                    ApiResponse.Fail(
                        message: "The list of active ingredients cannot be empty."));
            }

            return FromResponse(
                await _productService.AddProductActiveIngredientsAsync(productId, dtos));
        }
        
        [HttpPost("{productId}/active-ingredients/{activeIngredientId}")]
        public async Task<IActionResult> AddProductActiveIngredientAsync(
            int productId,
            int activeIngredientId, 
            ProductActiveIngredientDto dto)
        {
            return FromResponse(
                await _productService.AddProductActiveIngredientAsync(productId, activeIngredientId, dto));
        }



        [HttpGet("{productId}/active-ingredients")]
        public async Task<IActionResult> GetProductActiveIngredientsAsync(int productId)
        {
            return FromResponse(
                await _productService.GetProductActiveIngredientsAsync(productId));
        }



        [HttpPut("{productId}/active-ingredients")]
        public async Task<IActionResult> UpdateProductActiveIngredientsAsync(
            int productId,
            List<ProductActiveIngredientAssociationDto> dtos)
        {
            if (dtos.Count == 0)
            {
                return FromResponse(
                    ApiResponse.Fail(
                        message: "The list of active ingredients cannot be empty."));
            }

            return FromResponse(
                await _productService.UpdateProductActiveIngredientsAsync(productId, dtos));
        }

        [HttpPut("{productId}/active-ingredients/{activeIngredientId}")]
        public async Task<IActionResult> UpdateProductActiveIngredientAsync(
            int productId,
            int activeIngredientId,
            ProductActiveIngredientDto dto)
        {
            return FromResponse(
                await _productService.UpdateProductActiveIngredientAsync(productId, activeIngredientId, dto));
        }



        [HttpDelete("{productId}/active-ingredients")]
        public async Task<IActionResult> DeleteProductActiveIngredientsAsync(int productId)
        {
            return FromResponse(
                await _productService.DeleteProductActiveIngredientsAsync(productId));
        }

        [HttpDelete("{productId}/active-ingredients/{activeIngredientId}")]
        public async Task<IActionResult> DeleteProductActiveIngredientAsync(int productId, int activeIngredientId)
        {
            return FromResponse(
                await _productService.DeleteProductActiveIngredientAsync(productId, activeIngredientId));
        }




        [HttpPost("{productId}/packaging")]
        public async Task<IActionResult> AddProductPackagingAsync(int productId, ProductPackagingDto dto)
        {
            return FromResponse(
                await _productPackagingService.CreateAsync(productId, dto));
        }

        [HttpGet("{productId}/packaging")]
        public async Task<IActionResult> GetAllProductPackagingAsync(int productId)
        {
            return FromResponse(
                await _productPackagingService.GetProductPackagingLevelsAsync(productId));
        }

        [HttpPut("{productId}/packaging/{productPackagingId}")]
        public async Task<IActionResult> UpdateProductPackagingAsync(
            int productId, 
            int productPackagingId, 
            [FromBody] UpdateProductPackagingDto dto)
        {
            return FromResponse(
                await _productPackagingService.UpdateAsync(productId, productPackagingId, dto));
        }

        [HttpDelete("{productId}/packaging")]
        public async Task<IActionResult> DeleteProductPackagingLevelsAsync(int productId)
        {
            return FromResponse(
                await _productPackagingService.DeleteProductPackagingLevelsAsync(productId));
        }

        [HttpDelete("{productId}/packaging/{productPackagingId}")]
        public async Task<IActionResult> DeleteProductPackagingAsync(int productId, int productPackagingId)
        {
            return FromResponse(
                await _productPackagingService.DeleteAsync(productId, productPackagingId));
        }

        [HttpGet("{productId}/packaging/base-unit")]
        public async Task<IActionResult> GetProductBaseUnitAsync(int productId)
        {
            return FromResponse(
                await _productPackagingService.GetProductBaseUnit(productId));
        }

        [HttpPut("{productId}/default-packaging/{productPackagingId}")]
        public async Task<IActionResult> AddProductDefaultPackagingAsync(int productId, int productPackagingId)
        {
            return FromResponse(
                await _productPackagingService.AddProductDefaultPackagingAsync(productId, productPackagingId));
        }

        [HttpGet("{productId}/default-packaging")]
        public async Task<IActionResult> GetProductDefaultPackagingAsync(int productId)
        {
            return FromResponse(
                await _productPackagingService.GetProductDefaultPackagingAsync(productId));
        }

        [HttpDelete("{productId}/default-packaging")]
        public async Task<IActionResult> DeleteProductDefaultPackagingAsync(int productId)
        {
            return FromResponse(
                await _productPackagingService.DeleteProductDefaultPackagingAsync(productId));
        }
    }
}
