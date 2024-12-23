using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Threading.Tasks;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        #region Movie CRUD

        // Yeni film ekleme sayfasını render et
        public IActionResult CreateMovie()
        {
            return View();
        }

        // Film ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> CreateMovie(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPanel");
            }

            return View(movie);
        }

        // Film düzenleme sayfasını render et
        public async Task<IActionResult> EditMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // Film düzenleme işlemi
        [HttpPost]
        public async Task<IActionResult> EditMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Movies.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Movies.Any(e => e.Id == movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("AdminPanel");
            }
            return View(movie);
        }

        // Film silme işlemi
        public async Task<IActionResult> DeleteMovie(int id)
        {
            return await DeleteEntity<Movie>(id);
        }

        #endregion

        #region TVShow CRUD

        // Yeni dizi ekleme sayfasını render et
        public IActionResult CreateTVShow()
        {
            return View();
        }

        // Dizi ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> CreateTVShow(TVShow tvShow)
        {
            if (ModelState.IsValid)
            {
                _context.TVShows.Add(tvShow);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPanel");
            }

            return View(tvShow);
        }

        // Dizi düzenleme sayfasını render et
        public async Task<IActionResult> EditTVShow(int id)
        {
            var tvShow = await _context.TVShows.FindAsync(id);
            if (tvShow == null)
            {
                return NotFound();
            }
            return View(tvShow);
        }


        // Dizi düzenleme işlemi
        [HttpPost]
        public async Task<IActionResult> EditTVShow(int id, TVShow tvShow)
        {
            if (id != tvShow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.TVShows.Update(tvShow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TVShows.Any(e => e.Id == tvShow.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("AdminPanel");
            }
            return View(tvShow);
        }

        // Dizi silme işlemi
        public async Task<IActionResult> DeleteTVShow(int id)
        {
            return await DeleteEntity<TVShow>(id);
        }

        #endregion

        #region Comment CRUD

        // Yorum silme işlemi
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminPanel"); // Admin paneline yönlendir
        }


        #endregion

        #region User CRUD

        // Kullanıcı silme işlemi
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminPanel"); // Admin paneline yönlendir
        }


        #endregion

        #region Admin Panel

        // Admin Panel
        public async Task<IActionResult> AdminPanel()
        {
            var model = new
            {
                Movies = await _context.Movies.ToListAsync(),
                TVShows = await _context.TVShows.ToListAsync(),
                Comments = await _context.Comments.Include(c => c.Movie).Include(c => c.TVShow).ToListAsync(),
                Users = await _context.Users.ToListAsync()
            };

            return View(model);
        }

        #endregion

        #region Helper Methods

        // Delete işlemini tekrar etmek için ortak bir metot
        private async Task<IActionResult> DeleteEntity<T>(int id) where T : class
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminPanel");
        }

        #endregion
    }
}