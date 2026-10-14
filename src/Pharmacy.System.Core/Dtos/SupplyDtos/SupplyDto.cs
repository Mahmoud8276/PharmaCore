using Pharmacy.System.Core.Dtos.SupplyItemDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.SupplyDtos
{
    public class SupplyDto
    {
        [Range(1, 100, ErrorMessage = "Discount Percentage Must Be Within Range 1 : 100 !")]
        public decimal DiscountPercentage { get; set; } = 0;


        [Required(ErrorMessage = "Receivement Date Is Required!")]
        public DateOnly ReceivedAt { get; set; }


        [Required(ErrorMessage = "Supplier Id Is Required!")]
        public int SupplierId { get; set; }


        [MinLength(1, ErrorMessage = "At Least One Supply Item Must Be Provided!")]
        public List<SupplyItemDto> Items { get; set; } = new List<SupplyItemDto>();
    }
}
