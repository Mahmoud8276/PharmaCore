using System;

namespace Pharmacy.System.Core.Models
{
    public class ProductBatch : BaseModel<int>
    {
        public string Number { get; set; } = null!;
        public DateOnly? ManufactureDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
