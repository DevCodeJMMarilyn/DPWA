using appweb2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using appweb2.filtros;
using Microsoft.EntityFrameworkCore;

namespace appweb2.Models
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existe = _context.Usuarios.Any(u => u.Email == model.Email);
            if (existe)
            {
                ViewBag.Error = "El correo ya está registrado";
                return View(model);
            }

            var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var saltedPassword = salt + model.Password;
            byte[] hashBytes;
            using (SHA256 sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            }

            var user = new Usuario
            {
                Nombre = model.Nombre,
                Email = model.Email,
                Salt = salt,
                Password = hashBytes,
                RoleId = GetOrCreateRoleId("User"),
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(user);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Login(Login model)
        {
            if (string.IsNullOrWhiteSpace(model.correo) || string.IsNullOrWhiteSpace(model.password))
            {
                ViewBag.Error = "Credenciales Incorrectas";
                return View("Index");
            }

            var user = _context.Usuarios
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == model.correo);
            if (user == null)
            {
                ViewBag.Error = "Credenciales Incorrectas";
                return View("Index");
            }

            var saltedPassword = user.Salt + model.password;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytesUnicode = Encoding.Unicode.GetBytes(saltedPassword);
                byte[] hashBytesUnicode = sha256.ComputeHash(inputBytesUnicode);

                byte[] inputBytesUtf8 = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hashBytesUtf8 = sha256.ComputeHash(inputBytesUtf8);

                if (hashBytesUnicode.SequenceEqual(user.Password) || hashBytesUtf8.SequenceEqual(user.Password))
                {
                    HttpContext.Session.SetString("usuario", user.Nombre);
                    HttpContext.Session.SetInt32("usuarioId", user.Id);
                    var roleNombre = user.Role != null ? user.Role.Nombre : string.Empty;
                    HttpContext.Session.SetString("rol", roleNombre);

                    if (string.Equals(roleNombre, "Admin", StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction(nameof(Dashboard));

                    return RedirectToAction("Index", "VideoJuegos");
                }
            }

            ViewBag.Error = "Credenciales Incorrectas";
            return View("Index");
        }

        [HttpGet]
        [AdminAuthorize]
        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel
            {
                Categorias = await ObtenerResumenCategoriasAsync()
            };

            return View(model);
        }

        [HttpGet]
        [AdminAuthorize]
        public async Task<IActionResult> ObtenerDatos(int? categoriaId)
        {
            var data = await ObtenerResumenCategoriasAsync(categoriaId);

            return Json(data.Select(x => new
            {
                categoriaId = x.CategoriaId,
                categoria = x.Categoria,
                total = x.Total
            }));
        }

        [HttpGet]
        [AdminAuthorize]
        public async Task<IActionResult> DetalleVentas(DateTime? desde, DateTime? hasta, int pagina = 1)
        {
            int paginador = 10;

            try
            {
                var query = _context.DetalleCompras
                    .AsNoTracking()
                    .Include(d => d.Compra)
                    .ThenInclude(c => c.VideoJuego)
                    .AsQueryable();

                if (desde.HasValue)
                {
                    query = query.Where(d => d.fechaHoraTransaccion >= desde.Value);
                }

                if (hasta.HasValue)
                {
                    query = query.Where(d => d.fechaHoraTransaccion <= hasta.Value);
                }

                var totalregistros = await query.CountAsync();

                var datos = await query
                    .OrderByDescending(d => d.fechaHoraTransaccion)
                    .Skip((pagina - 1) * paginador)
                    .Take(paginador)
                    .Select(d => new VentaViewModel
                    {
                        idCompra = d.idCompra,
                        UsuarioId = d.Compra != null ? d.Compra.UsuarioID : 0,
                        VideoJuegoID = d.Compra != null ? d.Compra.VideoJuegoID : (int?)null,
                        VideoJuegoTitulo = d.Compra != null && d.Compra.VideoJuego != null ? d.Compra.VideoJuego.Titulo : null,
                        VideoJuegosId = d.VideoJuegosId,
                        cantidad = d.cantidad,
                        total = d.total,
                        estadoCompra = d.estadoCompra,
                        fechaHoraTransaccion = d.fechaHoraTransaccion,
                        codigoTransaccion = d.codigoTransaccion
                    })
                    .ToListAsync();

                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalregistros / paginador);
                ViewBag.PaginaActual = pagina;
                ViewBag.Desde = desde;
                ViewBag.Hasta = hasta;

                return View(datos);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Message != null && ex.Message.Contains("detalle_compra"))
            {
                ViewBag.Error = "No se pudo consultar la tabla 'detalle_compra'. Verifica que exista en la base de datos y que la cadena de conexión apunte a la BD correcta.";
                ViewBag.TotalPaginas = 1;
                ViewBag.PaginaActual = 1;
                ViewBag.Desde = desde;
                ViewBag.Hasta = hasta;
                return View(Enumerable.Empty<VentaViewModel>());
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [SessionAuthorize]
        public async Task<IActionResult> MisCompras()
        {
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if (!usuarioId.HasValue)
                return RedirectToAction(nameof(Index));

            var compras = await _context.DetalleCompras
                .AsNoTracking()
                .Include(d => d.Compra)
                .ThenInclude(c => c.VideoJuego)
                .Where(d => d.Compra.UsuarioID == usuarioId.Value)
                .OrderByDescending(d => d.fechaHoraTransaccion)
                .Select(d => new CompraUsuarioViewModel
                {
                    CompraId = d.idCompra,
                    Juego = d.Compra.VideoJuego != null ? d.Compra.VideoJuego.Titulo : d.VideoJuegosId,
                    Cantidad = d.cantidad,
                    Total = d.total,
                    Estado = d.estadoCompra,
                    Fecha = d.fechaHoraTransaccion,
                    CodigoTransaccion = d.codigoTransaccion
                })
                .ToListAsync();

            for (int i = 0; i < compras.Count; i++)
            {
                compras[i].Numero = i + 1;
            }

            return View(compras);
        }

        private int GetOrCreateRoleId(string nombre)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Nombre == nombre);
            if (role != null)
                return role.Id;

            role = new Role { Nombre = nombre };
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role.Id;
        }

        private async Task<List<DashboardCategoriaViewModel>> ObtenerResumenCategoriasAsync(int? categoriaId = null)
        {
            var query = _context.Categorias
                .AsNoTracking()
                .Select(c => new DashboardCategoriaViewModel
                {
                    CategoriaId = c.Id,
                    Categoria = c.Nombre,
                    Total = c.VideoJuegos.Count()
                });

            if (categoriaId.HasValue)
            {
                query = query.Where(c => c.CategoriaId == categoriaId.Value);
            }

            return await query
                .OrderBy(c => c.Categoria)
                .ThenBy(c => c.CategoriaId)
                .ToListAsync();
        }
    }
}
