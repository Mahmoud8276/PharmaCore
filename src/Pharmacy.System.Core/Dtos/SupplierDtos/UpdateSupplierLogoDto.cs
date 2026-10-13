using Microsoft.AspNetCore.Http;
using Pharmacy.System.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.SupplierDtos
{
    public class UpdateSupplierLogoDto
    {
        [Required(ErrorMessage = "Supplier Logo Is Required!")]
        [AllowedImageTypes("png", "jpg", "jpeg", "webp")]
        public IFormFile Logo { get; set; } = null!;
    }
}
