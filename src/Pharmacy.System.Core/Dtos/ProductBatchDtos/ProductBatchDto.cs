using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.ProductBatchDtos
{
    public class ProductBatchDto : IValidatableObject
    {
        [Required(ErrorMessage = "Product batch number is required!")]
        public string Number { get; set; } = null!;
        
        public DateOnly? ManufactureDate { get; set; }

        [Required(ErrorMessage = "Expiry date is required!")]
        public DateOnly ExpiryDate { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(ManufactureDate.HasValue && ManufactureDate > ExpiryDate)
            {
                yield return new ValidationResult(
                    "Invalid dates!",
                    new string[] { nameof(ManufactureDate), nameof(ExpiryDate) });
            }
        }
    }
}
