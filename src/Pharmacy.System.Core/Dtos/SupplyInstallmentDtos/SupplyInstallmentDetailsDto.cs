using Pharmacy.System.Core.Dtos.SupplyPaymentDtos;
using Pharmacy.System.Core.Enums;
using System;
using System.Collections.Generic;

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

        public List<SupplyPaymentDetailsDto> Payments { get; set; } = new List<SupplyPaymentDetailsDto>();
    }
}
