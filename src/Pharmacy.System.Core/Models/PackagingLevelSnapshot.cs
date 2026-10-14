
namespace Pharmacy.System.Core.Models
{
    public class PackagingLevelSnapshot
    {
        public int LevelId { get; set; }
        public int PackagingUnitId { get; set; }
        public string UnitName { get; set; } = null!;
        public int? ParentId { get; set; } = null;
        public decimal QuantityOfChildPackage { get; set; }
        public decimal QuantityOfBaseUnit { get; set; }
    }
}
