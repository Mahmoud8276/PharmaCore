using Pharmacy.System.Core.Enums;
using System;

namespace Pharmacy.System.Core.Dtos.SupplyInstallmentDtos
{
    public class SupplyInstallmentDetailsDto
    {
        public int Id { get; set; }
        public int SupplyId { get; set; }
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateOnly DueDate { get; set; }
        public SupplyInstallmentStatus Status { get; set; }
    }
}
