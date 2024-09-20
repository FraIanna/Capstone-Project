using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Capstone___Api.DataLayer.Entities
{
    public class Hall
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public required string Name { get; set; }

        [Required, StringLength(255)]
        public required string Description { get; set; }

        [Required]
        public required int Capacity { get; set; }

        public List<Movie> Movies{ get; set; } = [];

        public List<HallSeat> Seats { get; set; } = [];

        [JsonIgnore]
        public List<Reservation> Reservations { get; set; } = [];
    }
}
