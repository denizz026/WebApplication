using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MovieTVShowController : Controller
    {
        private readonly AppDbContext _context;

        public MovieTVShowController(AppDbContext context)
        {
            _context = context;
        }

        // Filmleri ve TV şovlarını listele
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

        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("SearchResults", new MovieTVShowViewModel());
            }

            var movies = _context.Movies
                .Where(m => m.Title.Contains(query))
                .ToList();

            var tvShows = _context.TVShows
                .Where(t => t.Title.Contains(query))
                .ToList();

            var viewModel = new MovieTVShowViewModel
            {
                Movies = movies,
                TVShows = tvShows
            };

            return View("SearchResults", viewModel);  // Buradaki "SearchResults" view'in doğru olduğundan emin olun
        }


        // Movie detayları
        public IActionResult MovieDetail(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // TV Show detayları
        public IActionResult TVShowDetail(int id)
        {
            var tvShow = _context.TVShows.FirstOrDefault(t => t.Id == id);
            if (tvShow == null)
            {
                return NotFound();
            }

            return View(tvShow);
        }
    }
}
