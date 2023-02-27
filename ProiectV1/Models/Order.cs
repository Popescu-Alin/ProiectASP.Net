using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectV1.Models
{
    public class Order
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public double TotalSum { get; set; }

        [MinLength(10, ErrorMessage = "O adresa completa are mai mult de 10 caractere")]
        [Required(ErrorMessage = "Adresa este obligatorie")]
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public virtual Profile? User { get; set; }
        public virtual ICollection<ProductOrder>? ProductOrders { get; set; }
    }
}
