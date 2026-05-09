using CinemaManager.Models.Cinema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaManager.Controllers
{
    public class MoviesController : Controller
    {
        private readonly CinemaManagerContext _context;

        public MoviesController(CinemaManagerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(m => m.Producer)
                .ToListAsync();
            return View(movies);
        }

        public async Task<IActionResult> MoviesAndTheirProds()
        {
            var movies = await _context.Movies
                .Include(m => m.Producer)
                .ToListAsync();
            return View(movies);
        }

        
        public IActionResult MoviesAndTheirProds_UsingModel()
        {
            var result = (from m in _context.Movies
                          join p in _context.Producers
                          on m.ProducerId equals p.Id
                          select new ProdMovie
                          {
                              mTitle = m.Title,
                              mGenre = m.Genre,
                              pName  = p.Name,
                              pNat   = p.Nationality
                          }).ToList();

            return View(result);
        }

        
        public IActionResult MyMovies(int id)
        {
            var result = (from m in _context.Movies
                          join p in _context.Producers
                          on m.ProducerId equals p.Id
                          where p.Id == id
                          select new ProdMovie
                          {
                              mTitle = m.Title,
                              mGenre = m.Genre,
                              pName  = p.Name,
                              pNat   = p.Nationality
                          }).ToList();

            return View(result);
        }

        public IActionResult SearchByTitle(string? Critère)
        {
            var movies = (from m in _context.Movies
                          join p in _context.Producers on m.ProducerId equals p.Id
                          where string.IsNullOrEmpty(Critère) || m.Title.Contains(Critère)
                          select new ProdMovie
                          {
                              mTitle = m.Title,
                              mGenre = m.Genre,
                              pName  = p.Name,
                              pNat   = p.Nationality
                          }).ToList();

            return View(movies);
        }

        
        public IActionResult SearchByGenre(string? Critère)
        {
            var movies = (from m in _context.Movies
                          join p in _context.Producers on m.ProducerId equals p.Id
                          where string.IsNullOrEmpty(Critère) || m.Genre.Contains(Critère)
                          select new ProdMovie
                          {
                              mTitle = m.Title,
                              mGenre = m.Genre,
                              pName  = p.Name,
                              pNat   = p.Nationality
                          }).ToList();

            return View(movies);
        }

    
        public IActionResult SearchBy2(string? genre, string? title)
        {
            
            var genres = _context.Movies
                .Select(m => m.Genre)
                .Distinct()
                .ToList();
            genres.Insert(0, "All");
            ViewBag.Genres = genres;

            var movies = (from m in _context.Movies
                          join p in _context.Producers on m.ProducerId equals p.Id
                          where (string.IsNullOrEmpty(genre)  || genre == "All" || m.Genre.Contains(genre))
                             && (string.IsNullOrEmpty(title)  || m.Title.Contains(title))
                          select new ProdMovie
                          {
                              mTitle = m.Title,
                              mGenre = m.Genre,
                              pName  = p.Name,
                              pNat   = p.Nationality
                          }).ToList();

            return View(movies);
        }

        

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies
                .Include(m => m.Producer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        public IActionResult Create()
        {
            ViewBag.Producers = _context.Producers.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,ProducerId")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Producers = _context.Producers.ToList();
            return View(movie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            ViewBag.Producers = _context.Producers.ToList();
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,ProducerId")] Movie movie)
        {
            if (id != movie.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Producers = _context.Producers.ToList();
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies
                .Include(m => m.Producer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null) _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
