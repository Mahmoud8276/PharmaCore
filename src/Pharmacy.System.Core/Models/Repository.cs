namespace Pharmacy.System.Core.Models
{
    public class Repository : BaseModel<int>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public bool IsMain { get; set; }
    }
}
