using System;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.SupplyItemDtos
{
    public class SupplyItemDto
    {
        [Required(ErrorMessage = "Product Quantity Is Rquired!")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }
        
        
        [Required(ErrorMessage = "Product Unit Price Is Rquired!")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "Price must be greater than zero.")]
        public decimal PricePerUnit { get; set; }
        
        
        [Required(ErrorMessage = "Product Expiry Date Is Rquired!")]
        public DateOnly ExpiryDate { get; set; }
        

        [Required(ErrorMessage = "Product Id Is Rquired!")]
        public int ProductId { get; set; }
        
        
        [Required(ErrorMessage = "Product Packaging Id Is Rquired!")]
        public int PackagingLevelId { get; set; }

    }
}
