using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectV1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string Title { get; set; }

        [MaxLength(100, ErrorMessage = "Lungimea maxima trebuie sa fie de 100 caractere")]
        [Required(ErrorMessage = "Descrierea produsului este obligatorie")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Pretul produsului este obligatoriu")]
        public double Price { get; set; }

        public double Stars { get; set; }

        public string? Photo { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int? CategoryId { get; set; }

        public Boolean Approved { get; set; }

        public string? UserId { get; set; }

        public virtual Profile? User { get; set; } //un produs este pus de un singur user

        public virtual Category? Category { get; set; }

        public virtual ICollection<Review>? Reviews { get; set; }

        public virtual ICollection<Cart>? Carts { get; set; }

        public virtual ICollection<ProductOrder>? Orders { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }
    }

}

