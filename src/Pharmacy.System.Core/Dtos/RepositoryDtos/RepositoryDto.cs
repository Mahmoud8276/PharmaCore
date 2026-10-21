using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Dtos.RepositoryDtos
{
    public class RepositoryDto
    {
        [Required(ErrorMessage = "Repository name is required!")]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
