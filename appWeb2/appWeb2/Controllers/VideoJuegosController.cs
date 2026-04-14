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

        public async Task<IActionResult> Admin()
        {
            var juegos = await _context.VideoJuegos
                .OrderByDescending(j => j.FechaRegistro)
                .ToListAsync();
                return View("admin", juegos);
            // return RedirectToAction("Index", "VideoJuegos");
        }
 
        // CRUD principal (usado en panel admin)

        // GET: /VideoJuegos/Index
        public async Task<IActionResult> Index()
        {
            var juegos = await _context.VideoJuegos
                .OrderByDescending(j => j.FechaRegistro)
                .ToListAsync();
            return View(juegos);
        }

        // GET: /VideoJuegos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /VideoJuegos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VideoJuegos juego, IFormFile? archivoImagen)
        {
            if (!ModelState.IsValid)
                return View(juego);

            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                juego.imagen = await GuardarImagen(archivoImagen);
            }

            juego.FechaRegistro = DateTime.Now;

            _context.VideoJuegos.Add(juego);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /VideoJuegos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var juego = await _context.VideoJuegos.FindAsync(id);

            if (juego == null)
                return NotFound();

            return View(juego);
        }

        // POST: /VideoJuegos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VideoJuegos juego, IFormFile? archivoImagen)
        {
            var juegoBD = await _context.VideoJuegos.FindAsync(id);

            if (juegoBD == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(juegoBD);

            // Actualizar campos
            juegoBD.titulo      = juego.titulo;
            juegoBD.precio      = juego.precio;
            juegoBD.categoria   = juego.categoria;
            juegoBD.descripcion = juego.descripcion;
            juegoBD.edadMinima  = juego.edadMinima;
            juegoBD.enPromocion = juego.enPromocion;

            // Si se subió nueva imagen, reemplazar la anterior
            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                EliminarImagen(juegoBD.imagen);
                juegoBD.imagen = await GuardarImagen(archivoImagen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /VideoJuegos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var juego = await _context.VideoJuegos.FindAsync(id);

            if (juego == null)
                return NotFound();

            return View(juego);
        }

        // POST: /VideoJuegos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var juego = await _context.VideoJuegos.FindAsync(id);

            if (juego != null)
            {
                EliminarImagen(juego.imagen);
                _context.VideoJuegos.Remove(juego);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Vistas de filtro (enlaces rápidos)

        // GET: /VideoJuegos/Nuevos → últimos 15 juegos por fecha
        [HttpGet]
        public async Task<IActionResult> Nuevos()
        {
            var juegos = await _context.VideoJuegos
                .OrderByDescending(j => j.FechaRegistro)
                .Take(15)
                .ToListAsync();

            return View(juegos);
        }

        // GET: /VideoJuegos/Promociones → juegos con enPromocion = true
        [HttpGet]
        public async Task<IActionResult> Promociones()
        {
            var juegos = await _context.VideoJuegos
                .Where(j => j.enPromocion)
                .ToListAsync();

            return View(juegos);
        }

        // GET: /VideoJuegos/Categorias → lista de categorías únicas
        [HttpGet]
        public async Task<IActionResult> Categorias()
        {
            var categorias = await _context.VideoJuegos
                .Select(j => j.categoria)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return View(categorias);
        }

        // GET: /VideoJuegos/PorCategoria?categoria=RPG
        [HttpGet]
        public async Task<IActionResult> PorCategoria(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return RedirectToAction(nameof(Categorias));

            var juegos = await _context.VideoJuegos
                .Where(j => j.categoria == categoria)
                .ToListAsync();

            ViewData["Categoria"] = categoria;

            // Reutiliza la misma vista que Nuevos (tarjetas de juegos)
            return View("PorCategoria", juegos);
        }

        // para imágenes

        private async Task<string> GuardarImagen(IFormFile archivo)
        {
            var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");

            // Crear la carpeta si no existe
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
            var rutaCompleta  = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return "/imagenes/" + nombreArchivo;
        }

        private void EliminarImagen(string? rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa))
                return;

            var rutaFisica = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot",
                rutaRelativa.TrimStart('/'));

            if (System.IO.File.Exists(rutaFisica))
                System.IO.File.Delete(rutaFisica);
        }
    }
}
