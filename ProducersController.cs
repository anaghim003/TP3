using CinemaManager.Models.Cinema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaManager.Controllers
{
    public class ProducersController : Controller
    {
        private readonly CinemaManagerContext _context;

        public ProducersController(CinemaManagerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var producers = await _context.Producers.ToListAsync();
            return View(producers);
        }

       

        public async Task<IActionResult> ProdsAndTheirMovies()
        {
            var producers = await _context.Producers
                .Include(p => p.Movies)
                .ToListAsync();
            return View(producers);
        }


        public IActionResult ProdsAndTheirMovies_UsingModel()
        {
            var result = (from p in _context.Producers
                          join m in _context.Movies
                          on p.Id equals m.ProducerId
                          select new ProdMovie
                          {
                              pName  = p.Name,
                              pNat   = p.Nationality,
                              mTitle = m.Title,
                              mGenre = m.Genre
                          }).ToList();

            return View(result);
        }

    

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var producer = await _context.Producers
                .FirstOrDefaultAsync(p => p.Id == id);
            if (producer == null) return NotFound();
            return View(producer);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Nationality,Email")] Producer producer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var producer = await _context.Producers.FindAsync(id);
            if (producer == null) return NotFound();
            return View(producer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Nationality,Email")] Producer producer)
        {
            if (id != producer.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(producer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var producer = await _context.Producers
                .FirstOrDefaultAsync(p => p.Id == id);
            if (producer == null) return NotFound();
            return View(producer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producer = await _context.Producers.FindAsync(id);
            if (producer != null) _context.Producers.Remove(producer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
