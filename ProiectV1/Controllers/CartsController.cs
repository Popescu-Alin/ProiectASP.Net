using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectV1.Data;
using ProiectV1.Models;

namespace ProiectV1.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<Profile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CartsController(ApplicationDbContext context,
                                    UserManager<Profile> userManager,
                                    RoleManager<IdentityRole> roleManager
                                    )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Index()
        {

            var CartProducts = db.Carts.Include("Product").Include("User").Include("Product.Category")
                                       .Where(cp => cp.UserId == _userManager.GetUserId(User) && cp.Product.Approved==true);

            var sum = CartProducts.Sum(x => x.Product.Price * x.Quantity);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            ViewBag.CartProducts = CartProducts;
            ViewBag.TotalPrice = sum;
            return View();
        }


        [Authorize(Roles = "User,Colaborator,Admin")]
        //Nu trebuie sa fac verificare pt utilizator
        public IActionResult New(int id)
        {
            
            var product = db.Products.Find(id);
            //verific daca exista produsul, daca da il adaug in cosul de cumparaturi
            if (product != null && product.Approved)
            {
                var cart = db.Carts.SingleOrDefault(cart => cart.ProductId == id && cart.UserId == _userManager.GetUserId(User));
                //daca nu mai exista acest produs in cos
                if (cart == null)
                {
                    Cart c = new Cart()
                    {
                        UserId = _userManager.GetUserId(User),
                        ProductId = id,
                        Quantity = 1
                    };

                    db.Carts.Add(c);
                    db.SaveChanges();
                }
                else
                {
                    var c = db.Carts.Find(cart.Id, cart.ProductId, cart.UserId);
                    c.Quantity += 1;
                    db.SaveChanges();
                }
                return Ok();
            }
            else
                return Redirect("/Products/Index");


        }


        [Authorize(Roles = "User,Colaborator,Admin")]
        
        public IActionResult Increase(int id, int ProductId)
        {

            var cart = db.Carts.Find(id, ProductId , _userManager.GetUserId(User));
            if (cart!=null)
            {
                cart.Quantity += 1;
                db.SaveChanges();
            }

            return RedirectToAction("Index","Carts");
        }
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Decrease(int id, int ProductId)
        {

            var cart = db.Carts.Find(id, ProductId, _userManager.GetUserId(User));
            if (cart != null)
            {   
                if(cart.Quantity > 1)
                    cart.Quantity -= 1;
                else
                    db.Carts.Remove(cart);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Carts");
        }


        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Delete(int id, int ProductId)
        {

            var cart = db.Carts.Find(id, ProductId, _userManager.GetUserId(User));
            if (cart != null)
            {
                db.Carts.Remove(cart);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Carts");
        }


    }
}
