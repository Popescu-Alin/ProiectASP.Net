using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProiectV1.Data;
using ProiectV1.Models;

namespace ProiectV1.Controllers
{
    public class ProfilesController : Controller
    {
        
        private readonly UserManager<Profile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfilesController(  UserManager<Profile> userManager,
                                    RoleManager<IdentityRole> roleManager
                                    )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {

            var profiles = _userManager.Users;
            var mydict = new Dictionary<Profile,IList<string>>();
            foreach (var profile in profiles)
            {
                mydict.Add(profile, _userManager.GetRolesAsync(profile).Result);
            }
           
            ViewBag.Profiles = mydict;
            ViewBag.test = profiles;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditRoll(string id) 
        {

            var profile = _userManager.FindByIdAsync(id).Result;

            if(profile ==null)
            {
                TempData["message"] = "Nu exista acest profil";
                return RedirectToAction("Index");
            }

            profile.AllRoles = GetAllRoles();
            profile.RoleId = _userManager.GetRolesAsync(profile).Result.First();

            return View(profile);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
      
        public async Task<IActionResult> EditRoll(string id,Profile requestProfile)
        {

          
           
            if(ModelState.IsValid)
            {
                var roles = await _userManager.GetRolesAsync(requestProfile);

                var newRole = _roleManager.FindByIdAsync(requestProfile.RoleId).Result;

                requestProfile = await _userManager.FindByIdAsync(id);

                await _userManager.RemoveFromRolesAsync(requestProfile, roles);
                await _userManager.AddToRoleAsync(requestProfile, newRole.Name);

                return RedirectToAction("Index");
                }
            else
            {
                requestProfile.AllRoles = GetAllRoles();
                return View(requestProfile);
            }
        }

        
        [NonAction]
        private List<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = _roleManager.Roles;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}
