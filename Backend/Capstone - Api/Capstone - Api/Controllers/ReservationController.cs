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
    public class ReservationController : ControllerBase
    {
        private readonly DataContext dataContext;

        public ReservationController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpPost("create/{movieId}")]
        public async Task<ActionResult> CreateReservation(int movieId, [FromBody] ReservationDto reservationDto)
        {
            var movie = await dataContext.Movies
                .Include(m => m.HallSeats)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null) 
            {
                return NotFound("movie not Found");
            }

            var hallSeat = movie.HallSeats.FirstOrDefault(s => !s.IsReserved);
            if (hallSeat == null)
            {
                return BadRequest("No available seats");
            }
            else if (hallSeat.IsReserved)
            {
                return BadRequest("Seat already reserved.");
            }

            var user = await dataContext.Users.FindAsync(reservationDto.UserId);
            var hall = await dataContext.Halls.FindAsync(reservationDto.HallId);
            if (user == null || hall == null)
            {
                return BadRequest("User or hall not found.");
            }

            var reservation = new Reservation
            {
                User = user,
                Movie = movie,
                MovieId = movie.Id,
                Seat = hallSeat,
                Hall = hall,
                CreatedAt = DateTime.Now,
                TicketPrice = 7.00M
            };

            hallSeat.IsReserved = true;
            
            dataContext.Reservations.Add(reservation);
            await dataContext.SaveChangesAsync();

            return Ok(reservation);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReservation(int userId)
        {
            var reservation = await dataContext.Reservations
                .Include(r => r.Movie)
                .Include(r => r.Hall)
                .Include(r => r.Seat)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            if (reservation == null || !reservation.Any())
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteReservation(int id)
        {
            var userId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            var reservation = await dataContext.Reservations
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId.ToString() == userId);

            if (reservation == null)
            {
                return NotFound(new { message = "Reservation not found or not authorized." });
            }

            var orders = await dataContext.Orders
                .Where(o => o.ReservationId == reservation.Id)
                .ToListAsync();

            if (orders.Any())
            {
                dataContext.Orders.RemoveRange(orders);
            }

            var hallSeat = await dataContext.HallSeats.FirstOrDefaultAsync(hs => hs.Id == reservation.HallSeatId);
            if (hallSeat != null)
            {
                hallSeat.IsReserved = false;
            }

            var hall = await dataContext.Halls.FirstOrDefaultAsync(h => h.Id == reservation.HallId);
            if (hall != null)
            {
                hall.Reservations.Remove(reservation);
            }

            var movie = await dataContext.Movies.FirstOrDefaultAsync(mov => mov.Id == reservation.MovieId);
            if (movie != null)
            {
                movie.Reservations.Remove(reservation);
            }

            dataContext.Reservations.Remove(reservation);
            await dataContext.SaveChangesAsync();

            return Ok(new { message = "Reservation deleted successfully." });
        }

    }
}
