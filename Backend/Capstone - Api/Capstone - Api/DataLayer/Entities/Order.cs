using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Capstone___Api.DataLayer.Entities
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public required Reservation Reservation { get; set; }

        public int UserId { get; set; }

        public bool IsDone { get; set; }

        public List<OrderItem> Items { get; set; } = [];
    }
}
