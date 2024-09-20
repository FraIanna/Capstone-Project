using Capstone___Api.Context;
using Capstone___Api.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Capstone___Api.DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Capstone___Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly DataContext dataContext;

        public ProductController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetProduct()
        {
            var products = await dataContext.Products.ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromForm]ProductDto request, IFormFile imageFile)
        {
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

            var product = new Product()
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Price = request.Price,
                ImagePath = request.ImagePath,
            };

            dataContext.Products.Add(product);
            await dataContext.SaveChangesAsync();

            return Ok(product);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetProductById(int id)
        {
            var product = await dataContext.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromForm]ProductDto request, IFormFile imageFile)
        {
            var product = await dataContext.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) {
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

                product.ImagePath = Path.Combine("images", fileName);
            }

            product.Name = request.Name;
            product.Price = request.Price;
            product.Category = request.Category;
            product.Description = request.Description;
            product.ImagePath = request.ImagePath;

            dataContext.Products.Update(product);
            await dataContext.SaveChangesAsync();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await dataContext.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            dataContext.Products.Remove(product);
            await dataContext.SaveChangesAsync();

            return Ok();
        }

    }
}
