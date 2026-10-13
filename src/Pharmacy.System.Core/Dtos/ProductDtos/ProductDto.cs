using Microsoft.AspNetCore.Http;
using Pharmacy.System.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.ProductDtos
{
    public class ProductDto
    {
        [Required(ErrorMessage = "Product Name is Required!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product Barcode is Required!")]
        public string Barcode { get; set; }
        public string? Description { get; set; }
        public double? MinimumStockLevel { get; set; }

        [AllowedImageTypes("png", "jpg", "jpeg", "webp")]
        public IFormFile? Image { get; set; }
        public int? ManufacturerId { get; set; }

        [Required(ErrorMessage = "Product Category is Required!")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Product From is Required!")]
        public int ProductFormId { get; set; }
        
    }
}
