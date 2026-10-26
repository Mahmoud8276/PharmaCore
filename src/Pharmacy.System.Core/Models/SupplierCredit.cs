using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.System.Core.Models
{
    public class SupplierCredit : BaseModel<int>
    {
        public decimal Amount { get; set; }
        public bool IsSetteld { get; set; }

        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;


        [ForeignKey("SupplyReturn")]
        public int SupplyReturnId { get; set; }
        public SupplyReturn SupplyReturn { get; set; } = null!;
    }
}
