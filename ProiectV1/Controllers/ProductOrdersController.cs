using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectV1.Data;
using ProiectV1.Models;

namespace ProiectV1.Controllers
{
    public class ProductOrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<Profile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProductOrdersController(ApplicationDbContext context,
                                    UserManager<Profile> userManager,
                                    RoleManager<IdentityRole> roleManager
                                    )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


    }
}
