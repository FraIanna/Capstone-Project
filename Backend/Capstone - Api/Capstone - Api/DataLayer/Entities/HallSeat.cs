using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Capstone___Api.DataLayer.Entities
{
    public class HallSeat
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int HallId { get; set; }

        public int? MovieId { get; set; }

        [ForeignKey("HallId")]
        [JsonIgnore]
        public Hall? Hall { get; set; }

        [Required]
        public required int SeatNumber { get; set; }

        public bool IsReserved { get; set; }

        [JsonIgnore]
        public List<Reservation> Reservations { get; set; } = [];

    }
}
