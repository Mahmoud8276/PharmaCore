
namespace Pharmacy.System.Core.Models
{
    public class ProductCategory : BaseModel<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}
