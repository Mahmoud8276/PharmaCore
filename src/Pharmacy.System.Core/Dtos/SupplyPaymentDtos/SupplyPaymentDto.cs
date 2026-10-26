using Pharmacy.System.Core.Attributes;
using Pharmacy.System.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.SupplyPaymentDtos
{
    public class SupplyPaymentDto
    {
        public string? Notes { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
            ErrorMessage = "Initial paid amount must be greater than zero.")]
        [Required(ErrorMessage = "Payment amount is required!")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment date is required!")]
        [NotInFuture(ErrorMessage = "Payment date can not be in the future!")]
        public DateOnly PaidAt { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.Other;
    }
}
