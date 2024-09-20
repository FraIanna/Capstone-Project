using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Capstone___Api.DataLayer.Entities
{
    public class Reservation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public required User User { get; set; }

        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public required Movie Movie { get; set; }

        public int HallSeatId { get; set; }

        [ForeignKey("HallSeatId")]
        public required HallSeat Seat { get; set; }

        public int HallId { get; set; }

        [ForeignKey("HallId")]
        public required Hall Hall { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public required decimal TicketPrice { get; set; } = 7.00M;
    }
}
