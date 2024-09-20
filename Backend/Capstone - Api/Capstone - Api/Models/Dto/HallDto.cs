using Capstone___Api.DataLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace Capstone___Api.Models.Dto
{
    public class HallDto
    {
        [Required, StringLength(100)]
        public required string Name { get; set; }

        [Required, StringLength(255)]
        public required string Description { get; set; }

        [Required]
        public required int Capacity { get; set; }
    }
}
