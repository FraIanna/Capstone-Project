using Capstone___Api.Context;
using Capstone___Api.DataLayer.Entities;
using Capstone___Api.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Capstone___Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HallController : ControllerBase
    {
        private readonly DataContext dataContext;

        public HallController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllHall() 
        {
            var halls = await dataContext.Halls
                .Include(h => h.Movies)
                .Include(h => h.Seats)
                .ToListAsync();
            
            return Ok(halls);
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateHall(HallDto request)
        {
            var hall = new Hall()
            {
                Name = request.Name,
                Description = request.Description,
                Capacity = request.Capacity
            };

            dataContext.Halls.Add(hall);
            await dataContext.SaveChangesAsync();

            return Ok(hall);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateHall(int id, [FromBody]HallDto request)
        {
            var hall = await dataContext.Halls
                .Include(h => h.Seats) 
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hall == null) {
                return NotFound($"Hall with ID {id} not found");
            }
            
            hall!.Name = request.Name;
            hall.Description = request.Description;
            hall.Capacity = request.Capacity;

            dataContext.Update(hall);
            await dataContext.SaveChangesAsync();

            return Ok(hall);
        }

        [HttpPost("add-seat/{id}")]
        public async Task<ActionResult> AddSeat(int id, [FromBody] List<HallSeatDto> HallSeatDto)
        {
            var hall = await dataContext.Halls
            .Include(h => h.Seats)
            .FirstOrDefaultAsync(h => h.Id == id);

            if (hall == null)
            {
                return NotFound($"Hall with ID {id} not found");
            }

            foreach (var seatDto in HallSeatDto)
            {
                var hallSeat = new HallSeat
                {
                    HallId = hall.Id,
                    SeatNumber = seatDto.SeatNumber,
                    IsReserved = seatDto.IsReserved
                };
                hall.Seats.Add(hallSeat);
            }

            dataContext.Halls.Update(hall);
            await dataContext.SaveChangesAsync();

            return Ok(hall);
        }
        
        [HttpGet("avaible-seats/{hallId}/{movieId}")]
        public async Task<IActionResult> GetAvailableSeats(int hallId, int movieId)
        {
            var movie = await dataContext.Movies
                .Include(m => m.HallSeats)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null)
            {
                return NotFound("Film non trovato.");
            }

            var hall = await dataContext.Halls
                .FirstOrDefaultAsync(h => h.Id == hallId);

            if (hall == null)
            {
                return NotFound("Sala non trovata.");
            }

            var seats = await dataContext.HallSeats
                .Where(s => s.HallId == hallId && !s.IsReserved)
                .ToListAsync();

            var availableSeatsForMovie = seats
                .Where(s => movie.HallSeats.Contains(s))
                .ToList();

            return Ok(availableSeatsForMovie);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetHallById(int id)
        {
            var hall = await dataContext.Halls
                .FirstOrDefaultAsync(h => h.Id == id);
            return Ok(hall);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHall(int id)
        {
            var hall = await dataContext.Halls
               .Include(h => h.Seats)
               .Include(h => h.Movies)
               .FirstOrDefaultAsync(h => h.Id == id);

            if (hall == null)
            {
                return NotFound($"Hall with ID {id} not found");
            }
             dataContext.Remove(hall);
            await dataContext.SaveChangesAsync();

            return Ok();
        }

    }
}
