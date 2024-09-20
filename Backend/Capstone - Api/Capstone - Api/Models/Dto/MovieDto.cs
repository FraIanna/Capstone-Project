using System.ComponentModel.DataAnnotations;

namespace Capstone___Api.Models.Dto
{
    public class MovieDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 1)]
        public required string Title { get; set; }

        [Required, StringLength(300)]
        public required string Description { get; set; }

        [Required]
        public required DateTime ReleaseDate { get; set; }

        [Required, StringLength(50)]
        public string? Category { get; set; }

        [Required]
        [Range(0, 360)]
        public required int Runtime { get; set; }

        public string? ImagePath { get; set; }

        public string? TrailerUrl { get; set; }

        public required List<MovieShowtimeDto> Showtimes { get; set; }

        public required List<string> Casts { get; set; }

        public int HallId { get; set; }
    }
}
