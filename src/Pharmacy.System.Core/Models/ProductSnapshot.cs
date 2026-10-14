using System.Collections.Generic;

namespace Pharmacy.System.Core.Models
{
    public class ProductSnapshot
    {
        public int productId { get; set; }
        public string Name { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string? Description { get; set; }
        public string? ManufacturerName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string ProductFormName { get; set; } = null!;
        public List<string> ActiveIngredientNames { get; set; } = new List<string>();
    }
}
