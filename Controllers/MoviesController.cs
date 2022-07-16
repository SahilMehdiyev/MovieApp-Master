using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Data;
using MovieApp.Web.Entity;
using MovieApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Controllers
{
    // controller
    public class MoviesController : Controller
    {
        private readonly MovieContext _context;
        public MoviesController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
       
        [HttpGet]
        public IActionResult List(int? id,string q)
        {
            var movies = _context.Movies.AsQueryable();

            if (id!=null)
            {
                movies = movies
                    .Include(m=> m.Genres)
                    .Where(m => m.Genres.Any(g=>g.GenreId==id));
            }

            if(!string.IsNullOrEmpty(q))
            {
                movies = movies.Where(i => 
                    i.Title.ToLower().Contains(q.ToLower()) || 
                    i.Description.ToLower().Contains(q.ToLower()));
            }

            var model = new MoviesViewModel()
            {
                Movies = movies.ToList()
            };
            
            return View("Movies", model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            return View(_context.Movies.Find(id));
        }

    }
}
