namespace Capstone___Api.Models.Dto
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
     
        public required string Email { get; set; }
     
        public required string Token { get; set; }
     
        public DateTime TokenExpiration { get; set; }
    }
}
