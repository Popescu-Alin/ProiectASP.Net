using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectV1.Data;
using ProiectV1.Models;

namespace ProiectV1.Controllers
{
    public class ReviewsController : Controller
    {
        
        private readonly ApplicationDbContext db;
        private readonly UserManager<Profile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ReviewsController(ApplicationDbContext context,
                                    UserManager<Profile> userManager,
                                    RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User,Colaborator")]
        public IActionResult Delete(int id)
        {
            Review review = db.Reviews.Find(id);
            if (review != null) { 
                if (review.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin")) {
                    db.Reviews.Remove(review);
                    db.SaveChanges();
                    return Redirect("/Products/Show/" + review.ProductId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa stergeti acest review";
                    return RedirectToAction("Index", "Products");
                }
            }
            else
            {
                TempData["message"] = "Nu exista acest review";
                return RedirectToAction("Index", "Products");
            }
        }

        [Authorize(Roles = "Admin,User,Colaborator")]
        public IActionResult Edit(int id)
        {
            Review review = db.Reviews.Find(id);
            if (review != null)
            {
                if (review.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    return View(review);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa editati acest review";
                    return RedirectToAction("Index", "Products");
                }
            }
            else
            {
                TempData["message"] = "Nu exista acest review";
                return RedirectToAction("Index", "Products");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User,Colaborator")]
        public IActionResult Edit(int id, Review requestReview)
        {

            Review review = db.Reviews.Find(id);
            if (review != null)
            {
                if (review.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    try
                    {
                        review.Content = requestReview.Content;
                        db.SaveChanges();
                        return Redirect("/Products/Show/" + review.ProductId);
                    }
                    catch (Exception e)
                    {
                        return View(requestReview);
                    }
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa editati acest review";
                    return RedirectToAction("Index", "Products");
                }
            }
            else
            {
                TempData["message"] = "Nu exista acest review";
                return RedirectToAction("Index", "Products");
            }

        }

    }
}
