using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProiectV1.Data;
using ProiectV1.Models;

namespace ProiectV1.Controllers
{
    //[Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<Profile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IWebHostEnvironment _env;


        public ProductsController(  ApplicationDbContext context,
                                    UserManager<Profile> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    IWebHostEnvironment env)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }




        //[Authorize(Roles ="User,Colaborator,Admin")]
        public IActionResult Index()
        {   


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            Privileges();

            var products = db.Products.Include("Category").Include("User")
                                      .Where(prod => prod.Approved == true);

            //                                         MOTOR DE CAUTARE

            var search = "";


            //cautare in continut sau titlu
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere

                products = db.Products.Include("Category")
                                      .Include("User")
                                      .Where(prod => prod.Approved == true && ( prod.Title.Contains(search)
                                                                             || prod.Description.Contains(search) ) );
            }

            ViewBag.SearchString = search;

            //                                   Sortare Pret+ stele
            var sortPret = Convert.ToString(HttpContext.Request.Query["sortPret"]);
            var sortStele = Convert.ToString(HttpContext.Request.Query["sortStele"]);
            //verific daca cele 2 criteri sunt valide.
            if (sortPret != "asc" && sortPret != "desc")
                sortPret = "";
            if (sortPret != "asc" && sortPret != "desc")
                sortPret = "";
            
            //se sorteaza dupa criteriul sp*Price and ss*Starts
            var sp = 0;
            var ss = 0;
            if ( sortPret =="asc")
                sp = 1;
            if (sortPret == "desc")
                sp = -1;

            if (sortStele == "asc")
                ss = 1;
            if (sortStele == "desc")
                ss = -1;

            products = products.OrderBy(prod => sp*prod.Price).ThenBy(prod=> ss*prod.Stars);

            ViewBag.sortPret = sortPret;
            ViewBag.sortStele = sortStele;





            //                                         paginare

            var perPage = 6;

            //teremin numarul paginii
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            //iau  perPage elemente
            int offset = currentPage* perPage;
            
            var pageProducts = products.Skip(offset).Take(perPage);

            //detrmin numarul de pagini;
            ViewBag.lastPage =  Math.Ceiling( (float)products.Count() / (float)perPage )-1;


            ViewBag.currentPage = currentPage;

            ViewBag.Products = pageProducts;



            if(search == ""  &&  sortStele =="" &&  sortPret=="")
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?page";
            } else
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?sortPret=" + sortPret + "&sortStele="+sortStele+"&search=" + search + "&page";
            }


            return View();
        }



        //[Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Show(int id)
        {
            var p=db.Products.Find(id);

            if (p == null)
            {
                TempData["message"] = "Produsul nu exista";
                return RedirectToAction("Index");
            }
            else
            {
            Product product = db.Products   .Include("Category")
                                            .Include("User")
                                            .Include("Reviews")
                                            .Include("Reviews.User")
                               .Where(prod => prod.Id == id)
                               .First();

                product.Stars = System.Math.Round(StarAvr(product.Id), 2);
                p.Stars = product.Stars;
                db.SaveChanges();
                //detremin daca e colaboratorului sau e admin si id-ul user-ului.
                Privileges();
                return View(product);
            }
        }



        //add review
        [HttpPost]
        //[Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show([FromForm] Review review)
        {

            review.Date = DateTime.Now;
            review.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                return Redirect("/Products/Show/" + review.ProductId);
            }
            else 
            {
                Product product = db.Products.Include("Category")
                                            .Include("User")
                                            .Include("Reviews")
                                            .Include("Reviews.User")
                               .Where(prod => prod.Id == review.ProductId)
                               .First();
                Privileges();
                return View(product);
            }
        }



        //calculeaz media stelelor
        private double StarAvr(int idProduct) {
            var reviews = db.Reviews.Where(rev => rev.ProductId == idProduct);
            if (reviews.Count()==0)
                return 0;
            double avr = 0;
            foreach(var rev in reviews)
                avr += rev.Stars;
            return avr / reviews.Count();
            
        }
        
        
        private void Privileges()
        {
            ViewBag.IsColaborator = User.IsInRole("Colaborator");
            ViewBag.IsAdmin = User.IsInRole("Admin");
            ViewBag.UserId = _userManager.GetUserId(User);
        }



        [Authorize(Roles ="Admin,Colaborator")]
        public IActionResult New()
        {
            Product product = new Product();
            product.Categ = GetAllCategories();
            return View(product);
        }



        // Se adauga articolul in baza de date
        [HttpPost]
        [Authorize(Roles = "Admin,Colaborator")]
        public async Task<IActionResult> New(Product product, IFormFile ProductPhoto)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            //daca exista poza
            if (ProductPhoto!=null) {
                var id = DateTime.Now.ToString("yymmssfff") + ProductPhoto.FileName;
                product.Photo =GetDatabaseFileName(id); 
                SaveImgToMemory(ProductPhoto,id);
            }
            else//nu am poza =>ma redirectioneaza la pagina de new
            {
                TempData["message"] = "Imaginea este obligatorie";
                product.Categ = GetAllCategories();
                return View(product);
            }
            //salvez poza in baza de date
            product.Stars = 0;
            product.UserId = _userManager.GetUserId(User);
            product.Approved = false;

            if(ModelState.IsValid)
            {   
                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost adaugat";
                return RedirectToAction("Index");


            }else 
            {
                product.Categ = GetAllCategories();
                return View(product);
            }
        }



        [Authorize(Roles = "Admin,Colaborator")]
        public IActionResult Edit(int id)
        {
            if (db.Products.Find(id) != null)
            {
                Product product = db.Products.Include("Category")
                                            .Where(art => art.Id == id)
                                            .First();

                product.Categ = GetAllCategories();
                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    return View(product);
                }
                else
                {
                    TempData["message"] = "Nu ameti dreptul sa faceti modificari asupra produsului";
                    return RedirectToAction("Index");
                }
            }else
            {
                TempData["message"] = "Nu exista acest produs";
                return RedirectToAction("Index");
            }
    
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin,Colaborator")]
        public async Task<IActionResult> Edit(int id, Product requestProduct, IFormFile ProductPhoto)
        {

            //salvez poza
            if (ProductPhoto != null)
            {
                var idPhoto = DateTime.Now.ToString("yymmssfff") + ProductPhoto.FileName;
                SaveImgToMemory(ProductPhoto, idPhoto);
                requestProduct.Photo = GetDatabaseFileName(idPhoto);
            }
          
            //fac editul
            var product = db.Products.Find(id);
            
            if (product==null)
            {
                TempData["message"] = "Nu exista acest produs";
                return RedirectToAction("Index");
            }

            try 
            {
                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin")) {
                    product.Title = requestProduct.Title;
                    product.Description = requestProduct.Description;
                    product.Price = requestProduct.Price;
                    if(requestProduct.Photo!=null)
                        product.Photo= requestProduct.Photo;
                    product.CategoryId = requestProduct.CategoryId;
                    
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost modificat";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu ameti dreptul sa faceti modificari asupra produsului";
                    return RedirectToAction("Index");
                }

            }
            catch(Exception e) 
            {
                requestProduct.Categ = GetAllCategories();
                return View(requestProduct);
            }
        }


        // Se sterge un articol din baza de date 
        [HttpPost]
        [Authorize(Roles = "Admin,Colaborator")]
        public ActionResult Delete(int id)
        {
            if (db.Products.Find(id) != null)
            {
                Product product = db.Products.Include("Reviews")
                              .Where(prod => prod.Id == id)
                              .First();



                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    DeleteImgFromMemory(product.Photo);
                    db.Products.Remove(product);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost sters";
                }
                else
                {
                    TempData["message"] = "Nu puteti sterge produslele altora";
                }
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu exista acest produs";
                return RedirectToAction("Index");
            }
        }



        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
           
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            return selectList;
        }



        [Authorize(Roles ="Admin")]
        public IActionResult IndexAdmin()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            Privileges();

            var products = db.Products.Include("Category").Include("User")
                                      .Where(prod => prod.Approved == false);

            
            //                                         MOTOR DE CAUTARE

            var search = "";
            //cautare in continut sau titlu
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere

                products = db.Products.Include("Category")
                                      .Include("User")
                                      .Where(prod => prod.Approved == false && (prod.Title.Contains(search)
                                                                             || prod.Description.Contains(search)));
            }

            ViewBag.SearchString = search;

            

            //                                         paginare

            var perPage = 6;

            //teremin numarul paginii
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            //iau  perPage elemente
            int offset = currentPage * perPage;

            var pageProducts = products.Skip(offset).Take(perPage);

            //detrmin numarul de pagini;
            ViewBag.lastPage = Math.Ceiling((float)products.Count() / (float)perPage)-1;


            ViewBag.currentPage = currentPage;

            ViewBag.Products = pageProducts;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Products/IndexAdmin/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Products/IndexAdmin/?page";
            }


            return View();
        }
        


        [Authorize(Roles = "Admin")]
        public IActionResult EditApprove(int id)
        {

            if (User.IsInRole("Admin"))
            {
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return RedirectToAction("IndexAdmin");
                }

                product.Approved = product.Approved==true? false:true;

                db.SaveChanges();
                if (product.Approved == true)
                {
                    TempData["message"] = "Produsul a fost aprobat";
                    return RedirectToAction("IndexAdmin");
                }
                else
                {
                    TempData["message"] = "Produsul a fost eliminat din lista produselor aprobate";
                    return RedirectToAction("Index");
                }
                

            }
            else
            {
                TempData["message"] = "Doar adinistratorul poate aproba/dezaproba produse";

                return RedirectToAction("Index");
            }

        }

        //Salveaza imaginea in memorie
        private async void SaveImgToMemory(IFormFile img, string id){

            
            var storagePath = Path.Combine(
                                         _env.WebRootPath, // Luam calea folderului wwwroot
                                         "images", // Adaugam calea folderului images
                                         id // Numele fisierului
                                        );
            
            // Uploadam fisierul la calea de storage
            using (var fileStream = new FileStream(storagePath, FileMode.Create))
            {
                await img.CopyToAsync(fileStream);
            }

        }

        // General calea de afisare a fisierului care va fi stocata in baza de date
        private string GetDatabaseFileName(string id)
        {
            var databaseFileName = "/images/" + id;
            return databaseFileName;
        }
        

        private void DeleteImgFromMemory(string id)
        {
            var storagePath = Path.Combine(
                                         _env.WebRootPath, // Luam calea folderului wwwroot
                                         "images", // Adaugam calea folderului images
                                         id // Numele fisierului
                                        );

            if (System.IO.File.Exists(storagePath)) 
                System.IO.File.Delete(storagePath);
            
         

        }
    }
}
