using System.ComponentModel.DataAnnotations;

namespace Capstone___Api.Models.Dto
{
    public class HallSeatDto
    {
        [Required]
        public required int SeatNumber { get; set; }

        public bool IsReserved { get; set; }
    }
}
