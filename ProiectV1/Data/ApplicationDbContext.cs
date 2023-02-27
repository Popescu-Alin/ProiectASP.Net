using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProiectV1.Models;

namespace ProiectV1.Data
{
    public class ApplicationDbContext : IdentityDbContext<Profile>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<Cart>()
            .HasKey(cp => new {
                cp.Id,
                cp.ProductId,
                cp.UserId
            });
            // definire relatii cu modelele Cart si Produs (FK)
            modelBuilder.Entity<Cart>()
            .HasOne(ab => ab.Product)
            .WithMany(ab => ab.Carts)
            .HasForeignKey(ab => ab.ProductId);
            // definire relatii cu modelele Cart si User (FK)
            modelBuilder.Entity<Cart>()
            .HasOne(ab => ab.User)
            .WithMany(ab => ab.Carts)
            .HasForeignKey(ab => ab.UserId);



            modelBuilder.Entity<ProductOrder>()
            .HasKey(cp => new {
                cp.ProductId,
                cp.OrderId
            });


            // definire relatii cu modelele ProductOrder si Produs (FK)
            modelBuilder.Entity<ProductOrder>()
            .HasOne(ab => ab.Product)
            .WithMany(ab => ab.Orders)
            .HasForeignKey(ab => ab.ProductId);
            // definire relatii cu modelele ProductOrder si Order (FK)
            modelBuilder.Entity<ProductOrder>()
            .HasOne(ab => ab.Order)
            .WithMany(ab => ab.ProductOrders)
            .HasForeignKey(ab => ab.OrderId);

        }
    }

}
