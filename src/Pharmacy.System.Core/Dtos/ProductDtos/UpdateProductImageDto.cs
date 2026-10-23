using Microsoft.AspNetCore.Http;
using Pharmacy.System.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.ProductDtos
{
    public class UpdateProductImageDto
    {
        [Required(ErrorMessage = "Product Image Is Required!")]
        [AllowedImageTypes("png", "jpg", "jpeg", "webp")]
        public IFormFile Image { get; set; } = null!;
    }
}
