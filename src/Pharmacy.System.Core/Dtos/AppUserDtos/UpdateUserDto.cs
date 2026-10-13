using Microsoft.AspNetCore.Http;
using Pharmacy.System.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.AppUserDtos
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
       
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number!")]
        public string? PhoneNumber { get; set; }

        [AllowedImageTypes("png", "jpg", "jpeg", "webp")]
        public IFormFile? UserImage { get; set; }
        public bool RemoveImage { get; set; } = false;
    }
}
