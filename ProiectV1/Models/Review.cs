using System.ComponentModel.DataAnnotations;

namespace ProiectV1.Models
{
    public class Review
    {
  
        [Key]
        public int Id { get; set; }

        [MaxLength(300, ErrorMessage = "Lungimea maxima trebuie sa fie de 300 caractere")]
        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int Stars { get; set; }
        public int? ProductId { get; set; }

        public string? UserId { get; set; }

        public virtual Profile? User { get; set; } //un review este pus de un singur user

        public virtual Product? Product { get; set; }
    }

}
