namespace Capstone___Api.Models.Dto
{
    public class OrderDto
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
