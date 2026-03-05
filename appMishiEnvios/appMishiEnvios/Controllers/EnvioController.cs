using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using appMishiEnvios.Data;
using appMishiEnvios.Models;

namespace appMishiEnvios.Controllers
{
    public class EnvioController : Controller
    {
        private readonly AppDbContext _context;

        public EnvioController(AppDbContext context)
        {
            _context = context;
        }

        // LISTA
        public IActionResult Index()
        {
            var envios = _context.Envios
                .Include(e => e.Destinatario)
                .ThenInclude(d => d.Cliente)
                .Include(e => e.EstadoEnvio)
                .ToList();

            return View(envios);
        }

        // CREATE GET
        public IActionResult Create()
        {
            ViewBag.DestinatarioId =
                new SelectList(_context.Destinatarios, "Id", "Nombre");

            ViewBag.EstadoEnvioId =
                new SelectList(_context.EstadosEnvio, "Id", "Nombre");

            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Envio envio)
        {
            _context.Envios.Add(envio);
            await _context.SaveChangesAsync();

            _context.Auditorias.Add(new Auditoria
            {
                Tabla = "Envios",
                Accion = "CREATE",
                Usuario = "Admin",
                Fecha = DateTime.Now,
                RegistroId = envio.Id
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var envio = await _context.Envios.FindAsync(id);

            ViewBag.DestinatarioId =
                new SelectList(_context.Destinatarios, "Id", "Nombre");

            ViewBag.EstadoEnvioId =
                new SelectList(_context.EstadosEnvio, "Id", "Nombre");

            return View(envio);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Envio envio)
        {
            if (id != envio.Id) return NotFound();

            _context.Update(envio);
            await _context.SaveChangesAsync();

            _context.Auditorias.Add(new Auditoria
            {
                Tabla = "Envios",
                Accion = "UPDATE",
                Usuario = "Admin",
                Fecha = DateTime.Now,
                RegistroId = envio.Id
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var envio = await _context.Envios
                .Include(e => e.Destinatario)
                .FirstOrDefaultAsync(e => e.Id == id);

            return View(envio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var envio = await _context.Envios.FindAsync(id);

            _context.Envios.Remove(envio);
            await _context.SaveChangesAsync();

            _context.Auditorias.Add(new Auditoria
            {
                Tabla = "Envios",
                Accion = "DELETE",
                Usuario = "Admin",
                Fecha = DateTime.Now,
                RegistroId = id
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}