using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.System.Core.Models
{
    public class Product : BaseModel<int>
    {
        public string Name { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string? Description { get; set; }
        public double? MinimumStockLevel { get; set; }
        public string? Image { get; set; }

        [ForeignKey("Manufacturer")]
        public int? ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }


        [ForeignKey("ProductCategory")]
        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }


        [ForeignKey("ProductForm")]
        public int ProductFormId { get; set; }
        public ProductForm ProductForm { get; set; }
        
        
        [ForeignKey("DefaultPackaging")]
        public int? DefaultPackagingId { get; set; }
        public ProductPackagingLevel? DefaultPackaging { get; set; }

        public ICollection<ProductActiveIngredient> ProductActiveIngredients { get; set; } = new HashSet<ProductActiveIngredient>();
        public ICollection<ProductPackagingLevel> ProductPackagingLevels { get; set; } = new HashSet<ProductPackagingLevel>();
    }
}
