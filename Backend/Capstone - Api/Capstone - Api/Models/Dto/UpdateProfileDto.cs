namespace Capstone___Api.Models.Dto
{
    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
