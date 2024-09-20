using System.ComponentModel.DataAnnotations;

namespace Capstone___Api.Models.Dto
{
    public class ProductDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Category { get; set; }

        [Required]
        [Range(0, 20)]
        public required decimal Price { get; set; }

        [Required]
        [StringLength(200)]
        public required string Description { get; set; }

        public string? ImagePath { get; set; }
    }
}
