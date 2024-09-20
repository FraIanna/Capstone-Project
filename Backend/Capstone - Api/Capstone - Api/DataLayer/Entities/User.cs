using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Capstone___Api.DataLayer.Entities
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 10)]
        public required string Password { get; set; }

        public List<Role> Roles { get; set; } = [];

        public List<Order> Orders { get; set; } = [];
    }
}