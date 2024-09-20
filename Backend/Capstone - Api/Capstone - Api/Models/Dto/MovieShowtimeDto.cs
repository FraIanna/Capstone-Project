using System.ComponentModel.DataAnnotations;

namespace Capstone___Api.Models.Dto
{
    public class MovieShowtimeDto
    {
        [Required]
        public required DateTime Start { get; set; }

        [Required]
        public required DateTime End { get; set; }
    }
}
