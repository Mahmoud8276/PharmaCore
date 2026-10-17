using Pharmacy.System.Core.Dtos.SupplyItemDtos;
using Pharmacy.System.Core.Enums;
using System;
using System.Collections.Generic;

namespace Pharmacy.System.Core.Dtos.SupplyDtos
{
    public class SupplyDetailsDto
    {
        public int Id { get; set; }
        public decimal TotalCost { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal FinalCost { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateOnly ReceivedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsConfirmed { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public List<SupplyItemDetailsDto> Items { get; set; } = new List<SupplyItemDetailsDto>();
    }
}
