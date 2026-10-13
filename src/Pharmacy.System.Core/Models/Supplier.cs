using System.Collections.Generic;

namespace Pharmacy.System.Core.Models
{
    public class Supplier : BaseModel<int>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Logo { get; set; }
        public List<SupplierPhoneNumber> PhoneNumbers { get; set; } = new List<SupplierPhoneNumber>();
    }

    public class SupplierPhoneNumber
    {
        public string Number { get; set; } = null!;
    }
}
