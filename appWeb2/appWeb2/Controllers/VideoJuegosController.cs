using appWeb2.Data;
using appWeb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace appWeb2.Controllers
{
    public class VideoJuegosController : Controller
    {
        private readonly AppDbContext _context;

        public VideoJuegosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var juegos = await _context.VideoJuegos.ToListAsync();
            return View(juegos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VideoJuegos juego)
        {
            if (!ModelState.IsValid)
                return View(juego);

            _context.VideoJuegos.Add(juego);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var juego = await _context.VideoJuegos.FindAsync(id);
            return View(juego);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var juego = await _context.VideoJuegos.FindAsync(id);

            _context.VideoJuegos.Remove(juego);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var juego = await _context.VideoJuegos.FindAsync(id);
            return View(juego);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, VideoJuegos juego)
        {
            _context.Update(juego);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
