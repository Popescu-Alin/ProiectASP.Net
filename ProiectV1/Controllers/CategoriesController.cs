using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectV1.Data;
using ProiectV1.Models;

namespace ProiectV1.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<Profile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CategoriesController(ApplicationDbContext context,
                                    UserManager<Profile> userManager,
                                    RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            ViewBag.Categories = categories;

            ViewBag.IsAdmin = User.IsInRole("Admin");


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            if(category == null)
            {
                TempData["message"] = "Categoria nu exista";
                return RedirectToAction("Index");
            }
            ViewBag.IsAdmin = User.IsInRole("Admin");
            category = db.Categories.Include("Products").Where(categ => categ.Id == id).First();
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {   
            
              return View();

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Category cat)
        {
           
                try
                {
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost modificata";
                    return RedirectToAction("Index");
                }

                catch (Exception e)
                {
                    return View(cat);
                }
            
            
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
           
            Category category = db.Categories.Find(id);
            if(category == null)
            {
                TempData["message"] = "Categoria nu exista";
                return RedirectToAction("Index");
            }
               
            return View(category);
            
          
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Category requestCategory)
        {         
            Category category = db.Categories.Find(id);
            if (category != null)
            {
                if (ModelState.IsValid)
                {
                    category.CategoryName = requestCategory.CategoryName;
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost modificata";

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCategory);
                }
            }
            else {
                TempData["message"] = "Categoria nu exista";
                return RedirectToAction("Index");
            }


        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {

            Category category = db.Categories.Find(id);
            if (category != null)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost stears";
            }
            else
            {
                TempData["message"] = "Categoria nu exista";
             
            }
            return RedirectToAction("Index");

        }
    }
}
