using Capstone___Api.DataLayer.Entities;

namespace Capstone___Api.Models.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public List<string>? Roles { get; set; }
    }
}
