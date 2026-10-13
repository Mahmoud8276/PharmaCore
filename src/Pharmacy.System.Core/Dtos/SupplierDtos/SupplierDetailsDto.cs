using Pharmacy.System.Core.Models;
using System.Collections.Generic;

namespace Pharmacy.System.Core.Dtos.SupplierDtos
{
    public class SupplierDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Logo { get; set; }
        public List<string> PhoneNumbers { get; set; } = new List<string>();
    }
}
