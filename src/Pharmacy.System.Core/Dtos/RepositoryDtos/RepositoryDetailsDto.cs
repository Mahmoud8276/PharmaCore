namespace Pharmacy.System.Core.Dtos.RepositoryDtos
{
    public class RepositoryDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public bool IsMain { get; set; }
    }
}
