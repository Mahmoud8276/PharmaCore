using System;

namespace Pharmacy.System.Core.Dtos.ProductBatchDtos
{
    public class ProductBatchDetailsDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public DateOnly? ManufactureDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
