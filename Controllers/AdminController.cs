using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Data;
using MovieApp.Web.Entity;
using MovieApp.Web.Identity;
using MovieApp.Web.Infrastructure;
using MovieApp.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly MovieContext _context;
        private UserManager<AppIdentityUser> _userManager;
        private IPasswordValidator<AppIdentityUser> _passwordValidator;
        private IPasswordHasher<AppIdentityUser> _passwordHasher;
        private RoleManager<IdentityRole> _roleManager;
        public AdminController(MovieContext context, UserManager<AppIdentityUser> userManager, IPasswordValidator<AppIdentityUser> passwordValidator, IPasswordHasher<AppIdentityUser> passwordHasher, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MovieUpdate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = _context.Movies.Select(m => new AdminEditMovieViewModel
            {
                MovieId = m.MovieId,
                Title = m.Title,
                Description = m.Description,
                ImageUrl = m.ImageUrl,
                GenreIds = m.Genres.Select(i=>i.GenreId).ToArray()
            }).FirstOrDefault(m => m.MovieId == id);

            ViewBag.Genres = _context.Genres.ToList();

            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        [HttpPost]
        public async Task<IActionResult> MovieUpdate(AdminEditMovieViewModel model, int[] genreIds, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var entity = _context.Movies.Include("Genres").FirstOrDefault(m => m.MovieId == model.MovieId);

                if (entity == null)
                {
                    return NotFound();
                }

                entity.Title = model.Title;
                entity.Description = model.Description;

                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName); // .jpg, .png
                    var fileName = string.Format($"{Guid.NewGuid()}{extension}");
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);
                    entity.ImageUrl = fileName;

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                entity.Genres = genreIds.Select(id => _context.Genres.FirstOrDefault(i => i.GenreId == id)).ToList();

                _context.SaveChanges();

                return RedirectToAction("MovieList");
            }
            ViewBag.Genres = _context.Genres.ToList();
            return View(model);
        }

        public IActionResult MovieList()
        {
            return View(new AdminMoviesViewModel
            {
                Movies = _context.Movies
                .Include(m => m.Genres)
                .Select(m => new AdminMovieViewModel
                {
                    MovieId = m.MovieId,
                    Title = m.Title,
                    ImageUrl = m.ImageUrl,
                    Genres = m.Genres.ToList()
                })
                .ToList()
            });
        }

        public IActionResult GenreList()
        {
            return View(GetGenres());
        }

        private AdminGenresViewModel GetGenres()
        {
            return new AdminGenresViewModel
            {
                Genres = _context.Genres.Select(g => new AdminGenreViewModel
                {
                    GenreId = g.GenreId,
                    Name = g.Name,
                    Count = g.Movies.Count
                }).ToList()
            };
        }

        [HttpPost]
        public IActionResult GenreCreate(AdminGenresViewModel model)
        {
            if(model.Name != null && model.Name.Length<3)
            {
                ModelState.AddModelError("Name", "tür adı minimum 3 karakterli olmalıdır.");
            }

            if(ModelState.IsValid)
            {
                _context.Genres.Add(new Genre { Name = model.Name });
                _context.SaveChanges();

                return RedirectToAction("GenreList");
            }
            return View("GenreList", GetGenres());

        }

        public IActionResult GenreUpdate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = _context
                .Genres
                .Select(g => new AdminGenreEditViewModel
                {
                    GenreId = g.GenreId,
                    Name = g.Name,
                    Movies = g.Movies.Select(i => new AdminMovieViewModel
                    {
                        MovieId = i.MovieId,
                        Title = i.Title,
                        ImageUrl = i.ImageUrl
                    }).ToList()
                }).FirstOrDefault(g => g.GenreId == id);

            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        [HttpPost]
        public IActionResult GenreUpdate(AdminGenreEditViewModel model, int[] movieIds)
        {
            if(ModelState.IsValid)
            {
                var entity = _context.Genres.Include("Movies").FirstOrDefault(i => i.GenreId == model.GenreId);
                if (entity == null)
                {
                    return NotFound();
                }

                entity.Name = model.Name;
                foreach (var id in movieIds)
                {
                    entity.Movies.Remove(entity.Movies.FirstOrDefault(m => m.MovieId == id));
                }

                _context.SaveChanges();

                return RedirectToAction("GenreList");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult GenreDelete(int genreId)
        {
            var entity = _context.Genres.Find(genreId);

            if (entity != null)
            {
                _context.Genres.Remove(entity);
                _context.SaveChanges();
            }

            return RedirectToAction("GenreList");
        }

        [HttpPost]
        public IActionResult MovieDelete(int movieId)
        {
            var entity = _context.Movies.Find(movieId);

            if (entity != null)
            {
                _context.Movies.Remove(entity);
                _context.SaveChanges();
            }

            return RedirectToAction("MovieList");
        }


        public IActionResult MovieCreate()
        {
            ViewBag.Genres = _context.Genres.ToList();
            return View(new AdminCreateMovieModel());
        }

        [HttpPost]
        public IActionResult MovieCreate(AdminCreateMovieModel model)
        {
            if (model.Title != null && model.Title.Contains("@"))
            {
                ModelState.AddModelError("", "Film başlığı @ işareti içeremez.");
            }

            //if(model.GenreIds == null)
            //{
            //    ModelState.AddModelError("GenreIds", "En az bir tür seçmelisiniz.");
            //}

            if (ModelState.IsValid)
            {
                var entity = new Movie
                {
                    Title = model.Title,
                    Description = model.Description,
                    ImageUrl = "no-image.png"
                };

                foreach (var id in model.GenreIds)
                {
                    entity.Genres.Add(_context.Genres.FirstOrDefault(i => i.GenreId == id));
                }

                _context.Movies.Add(entity);
                _context.SaveChanges();
                return RedirectToAction("MovieList", "Admin");
            }
            ViewBag.Genres = _context.Genres.ToList();
            return View(model);
        }

        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }

        public IActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserCreate(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                AppIdentityUser user = new AppIdentityUser();
                user.UserName = model.UserName;
                user.Email = model.Email;

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("UserList");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserDelete(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("UserList");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "user not found");
            }
            return View("UserList", _userManager.Users);
        }


        [HttpGet]
        public async Task<IActionResult> UserUpdate(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("UserList");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(string Id, string Password, string Email)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                user.Email = Email;

                IdentityResult validPass = null;

                if (!string.IsNullOrEmpty(Password))
                {
                    validPass = await _passwordValidator.ValidateAsync(_userManager, user, Password);

                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, Password);
                    }
                    else
                    {
                        foreach (var item in validPass.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }

                if (validPass.Succeeded)
                {
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserList");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }

            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }

            return View(user);
        }

        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleCreate(string name)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                }
               
            }
            
            return View(name);
        }

        [HttpPost]
        public async  Task<IActionResult> RoleDelete(string id)
        { 
            var role = await _roleManager.FindByIdAsync(id);
            if (role!=null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["message"] = $"{role.Name} has been deleted";
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
               
            }

            return RedirectToAction("RoleList");

        }

        public async Task<IActionResult> RoleEdit(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);

            var members = new List<AppIdentityUser>();
            var nonmembers = new List<AppIdentityUser>();

            foreach (var user in _userManager.Users.ToList())
            {
                
                    var list = await _userManager.IsInRoleAsync(user, role.Name)
                    ? members : nonmembers;
                list.Add(user);
            }

            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            IdentityResult result;

            if (ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);

                    if (user !=null)
                    {
                       result = await _userManager.AddToRoleAsync(user,model.RoleName);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }

                foreach (var userId in model.IdsToDelete ?? new string[]{ })
                {
                    var user = await _userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("RoleList");
            }
            else
            {
                return RedirectToAction("RoleEdit", model.RoleId);
            }
        }

    }
}
