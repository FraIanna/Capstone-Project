using Capstone___Api.Context;
using Capstone___Api.DataLayer.Entities;
using Capstone___Api.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Capstone___Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly DataContext dataContext;

        public OrderController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateOrder([FromBody] OrderDto orderDto) {

            var userId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            var reservation = await dataContext.Reservations
            .FirstOrDefaultAsync(r => r.UserId.ToString() == userId && r.Id == orderDto.ReservationId);
            if (reservation == null) {
                return BadRequest();
            }

            var order = new Order
            {
                UserId = Int32.Parse(userId!),
                Reservation = reservation,
                IsDone = false
            };

            dataContext.Orders.Add(order);
            await dataContext.SaveChangesAsync();

            var ticketOrderItem = new OrderItem
            {
                Order = order,
                Product = new Product
                {
                    Name = "Ticket",
                    Price = reservation.TicketPrice,
                    Category = "Ticket",
                    Description = "Biglietto"
                },
                Quantity = 1
            };
            dataContext.OrderItems.Add(ticketOrderItem);

            await dataContext.SaveChangesAsync();
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            var orders = await dataContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Reservation.UserId.ToString() == userId)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost("add-item")]
        public async Task<ActionResult> AddItemToOrder([FromBody] OrderItemDto orderItemDto)
        {
            var userId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            var order = await dataContext.Orders
                .Include(o => o.Reservation)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Reservation.UserId.ToString() == userId.ToString() && o.Id == orderItemDto.OrderId);

            if (order == null)
            {
                return BadRequest("Order not found.");
            }

            var product = await dataContext.Products.FindAsync(orderItemDto.ProductId);
            if (product == null)
            {
                return BadRequest(" Product not found. ");
            }

            var existingItem = order.Items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += orderItemDto.Quantity;
            }
            else
            {
                var orderItem = new OrderItem
                {
                    Order = order,
                    ProductId = product.Id,
                    Quantity = orderItemDto.Quantity,
                    Product = product
                };
                order.Items.Add(orderItem);
            }
            await dataContext.SaveChangesAsync();

            return Ok(order);
        }

        [HttpPut("update-item")]
        public async Task<ActionResult> UpdateOrderItem([FromBody] OrderItemDto orderItemDto)
        {
            var orderItem = await dataContext.OrderItems.FindAsync(orderItemDto.Id);

            if (orderItem == null)
            {
                return NotFound("Item not found.");
            }

            if (orderItemDto.Quantity == 0)
            {
                dataContext.OrderItems.Remove(orderItem);
            }
            else
            {
                orderItem.Quantity = orderItemDto.Quantity;
            }

            await dataContext.SaveChangesAsync();
            return Ok(orderItem);
        }

        //WORK IN PROGRESS

        //[HttpPost("confirm")]
        //public async Task<ActionResult> ConfirmOrder([FromBody] int orderId)
        //{
        //    var order = await dataContext.Orders
        //        .Include(o => o.Items)
        //        .ThenInclude(i => i.Product)
        //        .FirstOrDefaultAsync(o => o.Id == orderId);

        //    if (order == null || order.Items.Count == 0)
        //    {
        //        return BadRequest("Order is empty or does not exist.");
        //    }

        //    order.IsDone = true;
        //    await dataContext.SaveChangesAsync();

        //    return Ok("Order confirmed.");
        //}

        [HttpDelete]
        public async Task<ActionResult> DeleteItem(int itemId)
        {
            var userId = User.Claims
        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            var orderItem = await dataContext.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.Reservation)
                .FirstOrDefaultAsync(oi => oi.Id == itemId);

            if (orderItem == null)
            {
                return NotFound("Item not found.");
            }

            if (orderItem.Order.Reservation.UserId.ToString() != userId)
            {
                return Unauthorized("Need permission to delete.");
            }

            dataContext.OrderItems.Remove(orderItem);
            await dataContext.SaveChangesAsync();

            return Ok("Item deletes successfully.");
        }
    }
}
