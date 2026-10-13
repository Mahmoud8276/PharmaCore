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
        public List<SupplierPhoneNumberDto> PhoneNumbers { get; set; } = new List<SupplierPhoneNumberDto>();

    }

    public class SupplierPhoneNumberDto
    {
        [Required(ErrorMessage = "Supplier Phone Number Is Required")]
        [Phone]
        public string Number { get; set; } = null!;
    }
}
