using Pharmacy.System.Core.Dtos.ProductCategoryDtos;
using Pharmacy.System.Core.Dtos.ManufacturerDtos;
using Pharmacy.System.Core.Dtos.ProductFormDtos;
using System.Collections.Generic;

namespace Pharmacy.System.Core.Dtos.ProductDtos
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Barcodes { get; set; } = new List<string>();
        public string? Description { get; set; }
        public double? MinimumStockLevel { get; set; }
        public string? Image { get; set; }
        public ManufacturerDetailsDto Manufacturer { get; set; }
        public ProductCategoryDetailsDto Category { get; set; }
        public ProductFormDetailsDto ProductForm { get; set; }
        public List<ProductActiveIngredientDetailsDto> ActiveIngredients { get; set; } = new List<ProductActiveIngredientDetailsDto>();

    }
}
