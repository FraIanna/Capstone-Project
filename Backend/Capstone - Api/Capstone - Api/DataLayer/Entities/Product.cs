using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone___Api.DataLayer.Entities
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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
