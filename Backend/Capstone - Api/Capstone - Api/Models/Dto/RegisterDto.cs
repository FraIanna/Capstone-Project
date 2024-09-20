using System.ComponentModel.DataAnnotations;

namespace Capstone___Api.Models.Dto
{
    public class RegisterDto
    {
        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6)]
        public required string ConfirmPassword { get; set; }
    }
}
