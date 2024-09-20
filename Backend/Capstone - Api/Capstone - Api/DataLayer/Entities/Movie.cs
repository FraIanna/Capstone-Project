using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Capstone___Api.DataLayer.Entities
{
    public class Movie
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 1)]
        public required string Title { get; set; }

        [Required, StringLength(300)]
        public required string Description { get; set; }

        [Required]
        public required DateTime ReleaseDate { get; set; }

        [Required, StringLength(50)]
        public string ?Category { get; set; }

        [Required]
        [Range(0, 360)]
        public required int Runtime { get; set; }

        public string? ImagePath { get; set; }

        public string? TrailerUrl { get; set; }

        [StringLength(300)]
        public List<string>? Casts { get; set; } = [];

        public int HallId { get; set; }
        
        [JsonIgnore]
        public Hall? Hall { get; set; }

        [JsonIgnore]
        public List<MovieShowtime> MovieShowtimes { get; set; } = [];

        [JsonIgnore]
        public List<Reservation> Reservations { get; set; } = [];

        public List<HallSeat> HallSeats { get; set; } = [];
    }
}