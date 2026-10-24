using System;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.SupplyInstallmentDtos
{
    public class SupplyInstallmentDto
    {
        [Required(ErrorMessage = "Supply id is required!")]
        public int SupplyId { get; set; }
        public string? Notes { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
            ErrorMessage = "Amount must be greater than zero.")]
        [Required(ErrorMessage = "Installment Amount is required!")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Due date is required!")]
        public DateOnly DueDate { get; set; }

    }
}
