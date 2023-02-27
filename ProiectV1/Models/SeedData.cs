using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProiectV1.Data;

namespace ProiectV1.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificam daca in baza de date exista cel putin un rol
                // insemnand ca a fost rulat codul
                // De aceea facem return pentru a nu insera rolurile inca o data
                // Acesta metoda trebuie sa se execute o singura data
                if (context.Roles.Any())
                {
                    return; // baza de date contine deja roluri
                }
                // CREAREA ROLURILOR IN BD
                // daca nu contine roluri, acestea se vor crea
                context.Roles.AddRange(
                new IdentityRole { Id = "2d317444-3f6c-41d2-92be-6be179d4453d", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                new IdentityRole { Id = "2d317444-3f6c-41d2-92be-6be179d4453e", Name = "Colaborator", NormalizedName = "Colaborator".ToUpper() },
                new IdentityRole { Id = "2d317444-3f6c-41d2-92be-6be179d4453f", Name = "User", NormalizedName = "User".ToUpper() }
                );
                // o noua instanta pe care o vom utiliza pentru crearea parolelor utilizatorilor
                // parolele sunt de tip hash
                
                var hasher = new PasswordHasher<Profile>();
                
                // CREAREA USERILOR IN BD
                // Se creeaza cate un user pentru fiecare rol
                context.Users.AddRange(
                new Profile
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                    // primary key
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null,"Admin1!")
                },
                new Profile
                {
                   Id = "5d5e9c7f-4e48-4c04-8bfa-7b17eb7bd0bf",
                   // primary key
                   UserName = "colaborator@test.com",
                   EmailConfirmed = true,
                   NormalizedEmail = "COLABORATOR@TEST.COM",
                   Email = "colaborator@test.com",
                   NormalizedUserName = "COLABORATOR@TEST.COM",
                   PasswordHash = hasher.HashPassword(null,"Colaborator1!")
                },
                new Profile
                {
                    Id = "5d5e9c7f-4e48-4c04-8bfa-7b17eb7bd0bg",
                    // primary key
                    UserName = "user@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER@TEST.COM",
                    Email = "user@test.com",
                    NormalizedUserName = "USER@TEST.COM",
                    PasswordHash = hasher.HashPassword(null,"User1!")
                }
                );
                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "2d317444-3f6c-41d2-92be-6be179d4453d",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                },
               new IdentityUserRole<string>
               {
                   RoleId = "2d317444-3f6c-41d2-92be-6be179d4453e",
                   UserId = "5d5e9c7f-4e48-4c04-8bfa-7b17eb7bd0bf"
               },
               new IdentityUserRole<string>
               {
                   RoleId = "2d317444-3f6c-41d2-92be-6be179d4453f",
                   UserId = "5d5e9c7f-4e48-4c04-8bfa-7b17eb7bd0bg"
               }
                );
                context.SaveChanges();
            }
        }
    }
}
