using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var movies = _context.Movies.ToList(); 
            var tvShows = _context.TVShows.ToList(); 
            var viewModel = new MovieTVShowViewModel
            {
                Movies = movies,
                TVShows = tvShows
            };
            return View(viewModel);
        }
        
        public IActionResult Error()
        {
            return View(); 
        }
    }
}
