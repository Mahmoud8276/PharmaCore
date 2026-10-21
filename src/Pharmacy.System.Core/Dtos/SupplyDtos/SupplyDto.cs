using Pharmacy.System.Core.Dtos.SupplyItemDtos;
using Pharmacy.System.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.SupplyDtos
{
    public class SupplyDto
    {
        [Range(1, 100, ErrorMessage = "Discount Percentage Must Be Within Range 1 : 100 !")]
        public decimal? DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Supply payment plan is required!")]
        public SupplyPaymentPlan PaymentPlan { get; set; }


        [Required(ErrorMessage = "Receivement Date Is Required!")]
        public DateOnly ReceivedAt { get; set; }


        [Required(ErrorMessage = "Supplier Id Is Required!")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Repository Id Is Required!")]
        public int RepositoryId { get; set; }


        [MinLength(1, ErrorMessage = "At Least One Supply Item Must Be Provided!")]
        public List<SupplyItemDto> Items { get; set; } = new List<SupplyItemDto>();
    }
}
