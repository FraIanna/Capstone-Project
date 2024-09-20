using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Capstone___Api.DataLayer.Entities
{
    public class OrderItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public required Product Product { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public required Order Order { get; set; }

        public int Quantity { get; set; }
    }
}
