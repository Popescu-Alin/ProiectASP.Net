using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectV1.Data;
using ProiectV1.Models;
namespace ProiectV1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<Profile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OrdersController(ApplicationDbContext context,
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

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            var orders = db.Orders.Include("User");
            if (!User.IsInRole("Admin"))
            {
                orders = orders.Where(ord=>ord.UserId == _userManager.GetUserId(User));
            }

            ViewBag.Orders = orders;

            return View();
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Show(int id)
        {

            var order = db.Orders.Include("User").Where(ord=> ord.Id==id).FirstOrDefault();
            if(order==null)
            {
                TempData["message"] = "Nu exista acesta comanda";
                return Redirect("Index");
            }

            if (User.IsInRole("Admin") || order.UserId == _userManager.GetUserId(User))
            {
                var products = db.ProductOrders.Include("Product").Where(po => po.OrderId == id);
                ViewBag.products = products;
                ViewBag.Cancel = (order.Date.AddDays(1) > DateTime.Now);
                return View(order);
            }
            else
            {
                TempData["message"] = "Nu aveti drep de vizulalizare";
                return Redirect("Index");
            }
            
        }

        [Authorize(Roles = "Admin,User,Colaborator")]
        public IActionResult New()
        {
            if (!db.Carts.Include("Product").Where(cp => cp.UserId == _userManager.GetUserId(User) && cp.Product.Approved == true).Any())
            {
                TempData["message"] = "Nu puteti plasa o comanda goala";
                return RedirectToAction("Index", "Products");
            }
            Order order = new Order();
            order.UserId = _userManager.GetUserId(User);

            return View(order);
        }



        // Se adauga articolul in baza de date
        [HttpPost]
        [Authorize(Roles = "Admin,Colaborator,User")]
        public IActionResult New(Order order)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            if (ModelState.IsValid)
            {   

                var products = db.Carts.Include("Product").Where(cp => cp.UserId == _userManager.GetUserId(User) && cp.Product.Approved == true);
                if (products.Any())
                {

                    double total = products.Sum(prod=>prod.Quantity * prod.Product.Price);
                    order.TotalSum = total;
                    order.Date = DateTime.Now;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    foreach (var prod in products)
                    {
                        var prodOrd = new ProductOrder()
                        {
                            OrderId = order.Id,
                            ProductId = prod.ProductId,
                            Quantity = prod.Quantity
                        };

                        var cart = db.Carts.Find(prod.Id, prod.ProductId, _userManager.GetUserId(User));
                        if (cart != null)
                        {
                            db.Carts.Remove(cart);
                        }

                        db.ProductOrders.Add(prodOrd);     
                    }
                   
                    db.SaveChanges();
                    TempData["message"] = "Comanda a fost plasata";
                    return RedirectToAction("Index","Products");
                }
                else
                {
                    TempData["message"] = "Nu puteti plasa o comanda goala";
                    return RedirectToAction("Index", "Products");
                }


            }
            else
            {
                
                return View(order);
            }
        }

        [Authorize(Roles = "Admin,Colaborator,User")]
        public IActionResult Delete(int id)
        {

            var order = db.Orders.Find(id);

            if (order is null)
            {
                TempData["message"] = "Nu exista aceasta comanda";
                return RedirectToAction("Index","Orders");
            }

            if (order.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                TempData["message"] = "Nu aveti drepturi asupra aceste comenzi aceasta comanda";
                return RedirectToAction("Index", "Orders");
            }
            if (order.Date.AddDays(1) > DateTime.Now)
            {
                order = db.Orders.Include("ProductOrders").Where(ord => ord.Id == id).First();
                db.Orders.Remove(order);
                db.SaveChanges();
                TempData["message"] = "Comanda a fost anulata";
                return RedirectToAction("Index", "Orders");
            }
            else
            {

                TempData["message"] = "Nu mai putetoi anula comanda";
                return RedirectToAction("Index", "Orders");
            }
        }


    }
}
