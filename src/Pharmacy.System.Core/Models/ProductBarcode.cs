using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.System.Core.Models
{
    public class ProductBarcode : BaseModel<int>
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Barcode { get; set; } = null!;
    }
}
