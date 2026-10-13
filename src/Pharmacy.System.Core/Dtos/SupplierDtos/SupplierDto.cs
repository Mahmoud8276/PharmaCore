using Microsoft.AspNetCore.Http;
using Pharmacy.System.Core.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.SupplierDtos
{
    public class SupplierDto
    {
        [Required(ErrorMessage = "Supplier Name Is Required!")]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email!")]
        public string? Email { get; set; }
        public string? Address { get; set; }

        [AllowedImageTypes("png", "jpg", "jpeg", "webp")]
        public IFormFile? Logo { get; set; }
        
        [MinLength(1, ErrorMessage = "At least one phone number must be provided!")]
        public List<string> PhoneNumbers { get; set; } = new List<string>();

    }
}
