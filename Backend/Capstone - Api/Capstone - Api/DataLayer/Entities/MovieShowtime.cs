using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Capstone___Api.DataLayer.Entities
{
    public class MovieShowtime
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MovieId { get; set; }
        
        [JsonIgnore]
        public required Movie Movie { get; set; }
        
        [Required]
        public required DateTime Start { get; set; }

        [Required]
        public required DateTime End { get; set; }

    }
}
