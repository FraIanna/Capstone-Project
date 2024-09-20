using Capstone___Api.Context;
using Capstone___Api.DataLayer.Entities;
using Capstone___Api.Models.Dto;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Capstone___Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly string issuer;
        private readonly string audience;
        private readonly string key;

        public AuthController(DataContext dataContext, IConfiguration config)
        {
            this.dataContext = dataContext;
            issuer = config["Jwt:Issuer"]!;
            audience = config["Jwt:Audience"]!;
            key = config["Jwt:Key"]!;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterDto model) {
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password
            };

            dataContext.Users.Add(user);
            await dataContext.SaveChangesAsync();

            return Ok (model);
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody]LoginDto model)
        {
            var user = await dataContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Credentials not valid" });
            }

            Console.WriteLine("User roles: " + string.Join(", ", user.Roles.Select(r => r.Name)));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user!.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user!.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            user.Roles.ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));
            
            var k = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
            var signed = new SigningCredentials(k, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddMinutes(60);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: signed
            );
            return Ok(new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email,
                TokenExpiration = expiration,
                UserId = user.Id
            });
        }

        [HttpGet]
        [Route("Profile")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var claims = User.Claims.ToList();
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            var userClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;
            
            Console.WriteLine($"UserId: {userClaim}");

            if (string.IsNullOrEmpty(userClaim))
            {
                return Unauthorized(new { message = "Token invalid or non-existent" });
            }

            var user = await dataContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userClaim);

            if (user == null) {
                return NotFound(new {message = "User not found"});
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Roles = user.Roles.Select(r => r.Name).ToList(),
            };
            return Ok(userDto);
        }

        [HttpPut]
        [Route("Profile/Update")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            var userClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            if (string.IsNullOrEmpty(userClaim))
            {
                return Unauthorized(new { message = "Token invalid or non-existent" });
            }

            var user = await dataContext.Users.FindAsync(int.Parse(userClaim));

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!string.IsNullOrWhiteSpace(model.CurrentPassword) && user.Password != model.CurrentPassword)
            {
                return BadRequest(new { message = "Current Password is incorrect" });
            }

            user.Name = model.Name ?? user.Name;
            user.Email = model.Email ?? user.Email;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.Password = model.NewPassword;
            }

            dataContext.Users.Update(user);
            await dataContext.SaveChangesAsync();

            return Ok(new { message = "User data updated" });
        }

    }
}
