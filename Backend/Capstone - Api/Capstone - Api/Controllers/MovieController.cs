using Capstone___Api.Context;
using Capstone___Api.DataLayer.Entities;
using Capstone___Api.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Capstone___Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class MovieController : ControllerBase
    {
        private readonly DataContext dataContext;

        public MovieController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetAllMovie()
        {
            var movies = await dataContext.Movies
                .Include(m => m.Hall)
                .Include(m => m.MovieShowtimes)
                .ToListAsync();

            var movieDtos = movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                Category = m.Category,
                Runtime = m.Runtime,
                ImagePath = m.ImagePath,
                TrailerUrl = m.TrailerUrl,
                Casts = m.Casts!,
                Showtimes = m.MovieShowtimes.Select(ms => new MovieShowtimeDto
                {
                    Start = ms.Start,
                    End = ms.End
                }).ToList(),
                HallId = m.HallId
            }).ToList();

            return Ok(movieDtos);
        }

        [HttpPost]
        public async Task<ActionResult> CreateMovie([FromForm] MovieDto request, IFormFile imageFile)
        {
            var hall = await dataContext.Halls.FindAsync(request.HallId);
            if (hall == null)
            {
                return NotFound("Hall not found");
            }

            string? imagePath = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                imagePath = Path.Combine("images", fileName);
            }

            var movie = new Movie()
            {
                Title = request.Title,
                Description = request.Description,
                ReleaseDate = request.ReleaseDate,
                Category = request.Category,
                Runtime = request.Runtime,
                ImagePath = imagePath,
                TrailerUrl = request.TrailerUrl,
                Casts = request.Casts,
                Hall = hall
            };

            foreach (var showtimeDto in request.Showtimes)
            {
                movie.MovieShowtimes.Add(new MovieShowtime()
                {
                    Start = showtimeDto.Start,
                    End = showtimeDto.End,
                    MovieId = movie.Id,
                    Movie = movie
                });
            }

            dataContext.Movies.Add(movie);
            await dataContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllMovie), new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMovie(int id, [FromForm] MovieDto request, IFormFile imageFile)
        {
            var movie = await dataContext.Movies
                .Include(m => m.MovieShowtimes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                movie.ImagePath = Path.Combine("images", fileName);
            }

            movie.Title = request.Title;
            movie.Description = request.Description;
            movie.ReleaseDate = request.ReleaseDate;
            movie.Category = request.Category;
            movie.Runtime = request.Runtime;
            movie.ImagePath = request.ImagePath;
            movie.Casts = request.Casts;
            movie.Hall = dataContext.Halls.Find(request.HallId);

            movie.MovieShowtimes.Clear();
            movie.MovieShowtimes.AddRange(request.Showtimes.Select(s => new MovieShowtime
            {
                Start = s.Start,
                End = s.End,
                Movie = movie
            }));

            dataContext.Movies.Update(movie);
            await dataContext.SaveChangesAsync();

            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            var movie = await dataContext.Movies
                .Include(m => m.MovieShowtimes)
                .Include(m => m.Hall)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            dataContext.MovieShowtimes.RemoveRange(movie.MovieShowtimes);

            var hall = movie.Hall;
            if (hall != null)
            {
                hall.Movies.Remove(movie);
                dataContext.Update(hall);
            }

            dataContext.Movies.Remove(movie);
            await dataContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDto>> GetMovieById(int id)
        {
            var movie = await dataContext.Movies
                .Include(m => m.MovieShowtimes)
                .Include(m => m.Hall)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var movieDto = new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Category = movie.Category,
                Runtime = movie.Runtime,
                ImagePath = movie.ImagePath,
                TrailerUrl = movie.TrailerUrl,
                Showtimes = movie.MovieShowtimes.Select(st => new MovieShowtimeDto
                {
                    Start = st.Start,
                    End = st.End
                }).ToList(),
                Casts = movie.Casts!.ToList(),
                HallId = movie.HallId
            };

            return Ok(movieDto);
        }

    }
}
