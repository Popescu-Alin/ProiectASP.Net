using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectV1.Models
{
    public class Profile:IdentityUser
    {
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }


        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [NotMapped]
        public string RoleId { set; get; }
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
