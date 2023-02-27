using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectV1.Models
{
    public class Cart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string? UserId { get; set; }
        public int Quantity { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Profile? User { get; set; }
    }
}
