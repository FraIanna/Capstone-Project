namespace Capstone___Api.Models.Dto
{
    public class ReservationDto
    {
        public int UserId { get; set; } 
        public int MovieId { get; set; } 
        public int HallSeatId { get; set; } 
        public int HallId { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
