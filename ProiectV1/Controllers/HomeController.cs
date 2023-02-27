using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectV1.Data;
using ProiectV1.Models;
using System.Diagnostics;

namespace ProiectV1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext db;
        private readonly UserManager<Profile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context,
                              UserManager<Profile> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var products = from product in db.Products.Include("Category").Include("User").Where(prod=>prod.Approved==true)
                           orderby product.Stars descending
                           select product;

            ViewBag.FirstProductExists = false;
            if (products.Count()>0)
            {
                ViewBag.FirstProduct = products.First();
                ViewBag.FirstProductExists = true;
            }
            
            
            ViewBag.Products=products.Skip(1).Take(3);

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}