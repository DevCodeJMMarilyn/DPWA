using appweb2.Data;
using appweb2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Globalization;
using appweb2.filtros;
using System.Text.Json;
using System.Text;

namespace appweb2.Controllers
{
    public class VideoJuegosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private const string CarritoSessionKey = "carrito";

        public VideoJuegosController(AppDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var juegos = await _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .OrderBy(v => v.Titulo)
                .ToListAsync();
            return View(juegos);
        }

        public async Task<IActionResult> Categorias()
        {
            var categorias = await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Id)
                .ToListAsync();

            return View(categorias);
        }

        public async Task<IActionResult> PorCategoria(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(Categorias));

            var juegos = await _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .Where(v => v.CategoriaId == id.Value)
                .OrderBy(v => v.Titulo)
                .ToListAsync();

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id.Value);
            ViewData["CategoriaSeleccionada"] = categoria != null ? categoria.Nombre : string.Empty;
            return View("Index", juegos);
        }

        public async Task<IActionResult> NuevosJuegos()
        {
            var juegos = await _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .OrderByDescending(v => v.FechaRegistro)
                .Take(15)
                .ToListAsync();

            return View("Index", juegos);
        }

        public async Task<IActionResult> Promociones()
        {
            var juegos = await _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .Where(v => v.EnPromocion)
                .OrderBy(v => v.Titulo)
                .ToListAsync();

            return View("Index", juegos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juego = await _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (juego == null)
            {
                return NotFound();
            }

            var edadUsuario = HttpContext.Session.GetInt32("edad");
            if (edadUsuario.HasValue && edadUsuario.Value < juego.EdadMinima)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(juego);
        }

        [AdminAuthorize]
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(
                _context.Categorias
                    .AsNoTracking()
                    .OrderBy(c => c.Nombre)
                    .ThenBy(c => c.Id)
                    .ToList(),
                "Id",
                "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<IActionResult> Create(VideoJuego juego, IFormFile archivoImagen)
        {
            await CargarCategoriasAsync(juego.CategoriaId);
            ModelState.Remove(nameof(VideoJuego.Categoria));
            ModelState.Remove(nameof(VideoJuego.Imagen));
            ModelState.Remove(nameof(VideoJuego.FechaRegistro));

            var precioForm = Request.Form[nameof(VideoJuego.Precio)].ToString();
            if (!string.IsNullOrWhiteSpace(precioForm))
            {
                if (decimal.TryParse(precioForm, NumberStyles.Number, CultureInfo.InvariantCulture, out var precioInv) ||
                    decimal.TryParse(precioForm, NumberStyles.Number, CultureInfo.CurrentCulture, out precioInv))
                {
                    juego.Precio = precioInv;
                    ModelState.Remove(nameof(VideoJuego.Precio));
                }
            }

            var precioPromoForm = Request.Form[nameof(VideoJuego.PrecioPromocion)].ToString();
            if (string.IsNullOrWhiteSpace(precioPromoForm))
            {
                juego.PrecioPromocion = null;
                ModelState.Remove(nameof(VideoJuego.PrecioPromocion));
            }
            else
            {
                if (decimal.TryParse(precioPromoForm, NumberStyles.Number, CultureInfo.InvariantCulture, out var precioPromoInv) ||
                    decimal.TryParse(precioPromoForm, NumberStyles.Number, CultureInfo.CurrentCulture, out precioPromoInv))
                {
                    juego.PrecioPromocion = precioPromoInv;
                    ModelState.Remove(nameof(VideoJuego.PrecioPromocion));
                }
            }

            if (juego.EnPromocion)
            {
                if (juego.PrecioPromocion.HasValue && juego.PrecioPromocion.Value >= juego.Precio)
                    ModelState.AddModelError(nameof(VideoJuego.PrecioPromocion), "El precio de promoción debe ser menor al precio normal.");
            }
            else
            {
                juego.PrecioPromocion = null;
            }

            if (!await CategoriaExisteAsync(juego.CategoriaId))
                ModelState.AddModelError(nameof(VideoJuego.CategoriaId), "La categoría seleccionada no existe o ya no está disponible.");

            if (!ModelState.IsValid)
                return View(juego);

            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(archivoImagen.FileName)}";

                var ruta = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot", "imagenes", nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await archivoImagen.CopyToAsync(stream);
                }

                juego.Imagen = "/imagenes/" + nombreArchivo;
            }

            if (juego.FechaRegistro == default)
                juego.FechaRegistro = DateTime.Now;

            _context.VideoJuegos.Add(juego);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AdminAuthorize]
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var juego = await _context.VideoJuegos.FindAsync(id);
            if (juego == null)
            {
                return NotFound();
            }

            await CargarCategoriasAsync(juego.CategoriaId);
            return View(juego);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public async Task<IActionResult> AgregarAlCarrito(int id, int cantidad = 1, bool irAlCarrito = false)
        {
            var juego = await _context.VideoJuegos
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (juego == null)
                return NotFound();

            var carrito = ObtenerCarritoSession();
            var item = carrito.FirstOrDefault(x => x.VideoJuegoId == id);
            if (item == null)
            {
                carrito.Add(new CartSessionItem
                {
                    VideoJuegoId = id,
                    Cantidad = Math.Max(1, cantidad)
                });
            }
            else
            {
                item.Cantidad += Math.Max(1, cantidad);
            }

            GuardarCarritoSession(carrito);
            TempData["CarritoMensaje"] = $"{juego.Titulo} se agrego al carrito.";

            if (irAlCarrito)
                return RedirectToAction(nameof(Carrito));

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        [SessionAuthorize]
        public async Task<IActionResult> Carrito()
        {
            var model = await ConstruirCarritoAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public IActionResult ActualizarCarrito(int id, int cantidad)
        {
            var carrito = ObtenerCarritoSession();
            var item = carrito.FirstOrDefault(x => x.VideoJuegoId == id);
            if (item != null)
            {
                if (cantidad <= 0)
                    carrito.Remove(item);
                else
                    item.Cantidad = cantidad;

                GuardarCarritoSession(carrito);
            }

            return RedirectToAction(nameof(Carrito));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public IActionResult QuitarDelCarrito(int id)
        {
            var carrito = ObtenerCarritoSession();
            var item = carrito.FirstOrDefault(x => x.VideoJuegoId == id);
            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarritoSession(carrito);
                TempData["CarritoMensaje"] = "Juego eliminado del carrito.";
            }

            return RedirectToAction(nameof(Carrito));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize]
        public async Task<IActionResult> ProcesarCompra(CarritoViewModel model)
        {
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if (!usuarioId.HasValue)
                return RedirectToAction("Index", "Account");

            var carrito = await ConstruirCarritoAsync();
            if (!carrito.Items.Any())
            {
                TempData["CarritoError"] = "Tu carrito esta vacio.";
                return RedirectToAction(nameof(Carrito));
            }

            carrito.NumeroTarjeta = LimpiarTarjeta(model.NumeroTarjeta);
            carrito.NombreTitular = model.NombreTitular?.Trim() ?? string.Empty;
            carrito.ExpMonth = model.ExpMonth?.Trim() ?? string.Empty;
            carrito.ExpYear = model.ExpYear?.Trim() ?? string.Empty;
            carrito.Cvv = model.Cvv?.Trim() ?? string.Empty;

            if (!FormatoTarjetaValido(carrito.NumeroTarjeta))
                ModelState.AddModelError(nameof(model.NumeroTarjeta), "Ingresa una tarjeta valida.");

            if (!ExpiracionPareceValida(carrito.ExpMonth, carrito.ExpYear))
                ModelState.AddModelError(nameof(model.ExpYear), "La tarjeta esta vencida o la fecha no es valida.");

            if (!ModelState.IsValid)
                return View("Carrito", carrito);

            var descripcion = $"Compra carrito - {carrito.Items.Count} juego(s)";
            var pago = await ProcesarPagoSandboxConTarjetaAsync(
                carrito.NumeroTarjeta,
                carrito.ExpMonth,
                carrito.ExpYear,
                carrito.Cvv,
                carrito.Total,
                descripcion);
            if (!pago.aprobado)
            {
                TempData["CarritoError"] = pago.mensajeError;
                return RedirectToAction(nameof(Carrito));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in carrito.Items)
                {
                    var compra = new Compra
                    {
                        FechaCompra = DateTime.Now,
                        UsuarioID = usuarioId.Value,
                        VideoJuegoID = item.VideoJuegoId
                    };

                    _context.Compras.Add(compra);
                    await _context.SaveChangesAsync();

                    _context.DetalleCompras.Add(new DetalleCompra
                    {
                        VideoJuegosId = item.Titulo,
                        cantidad = item.Cantidad,
                        total = item.Subtotal,
                        estadoCompra = "Aprobado",
                        fechaHoraTransaccion = DateTime.Now,
                        codigoTransaccion = pago.codigoTransaccion,
                        idCompra = compra.Id
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["CarritoError"] = "El pago fue aprobado, pero ocurrio un problema al registrar la compra en la base de datos.";
                return RedirectToAction(nameof(Carrito));
            }

            GuardarCarritoSession(new List<CartSessionItem>());
            TempData["CarritoMensaje"] = $"Compra aprobada. Codigo de transaccion: {pago.codigoTransaccion}";
            return RedirectToAction("MisCompras", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<IActionResult> Edit(int id, VideoJuego juego, IFormFile? archivoImagen)
        {
            if (id != juego.Id)
                return NotFound();

            await CargarCategoriasAsync(juego.CategoriaId);
            ModelState.Remove(nameof(VideoJuego.Categoria));
            ModelState.Remove(nameof(VideoJuego.Imagen));
            ModelState.Remove(nameof(VideoJuego.FechaRegistro));

            var precioForm = Request.Form[nameof(VideoJuego.Precio)].ToString();
            if (!string.IsNullOrWhiteSpace(precioForm))
            {
                if (decimal.TryParse(precioForm, NumberStyles.Number, CultureInfo.InvariantCulture, out var precioInv) ||
                    decimal.TryParse(precioForm, NumberStyles.Number, CultureInfo.CurrentCulture, out precioInv))
                {
                    juego.Precio = precioInv;
                    ModelState.Remove(nameof(VideoJuego.Precio));
                }
            }

            var precioPromoForm = Request.Form[nameof(VideoJuego.PrecioPromocion)].ToString();
            if (string.IsNullOrWhiteSpace(precioPromoForm))
            {
                juego.PrecioPromocion = null;
                ModelState.Remove(nameof(VideoJuego.PrecioPromocion));
            }
            else
            {
                if (decimal.TryParse(precioPromoForm, NumberStyles.Number, CultureInfo.InvariantCulture, out var precioPromoInv) ||
                    decimal.TryParse(precioPromoForm, NumberStyles.Number, CultureInfo.CurrentCulture, out precioPromoInv))
                {
                    juego.PrecioPromocion = precioPromoInv;
                    ModelState.Remove(nameof(VideoJuego.PrecioPromocion));
                }
            }

            var juegoBD = await _context.VideoJuegos.FindAsync(id);
            if (juegoBD == null)
                return NotFound();

            if (juego.EnPromocion)
            {
                if (juego.PrecioPromocion.HasValue && juego.PrecioPromocion.Value >= juego.Precio)
                    ModelState.AddModelError(nameof(VideoJuego.PrecioPromocion), "El precio de promoción debe ser menor al precio normal.");
            }
            else
            {
                juego.PrecioPromocion = null;
            }

            if (!await CategoriaExisteAsync(juego.CategoriaId))
                ModelState.AddModelError(nameof(VideoJuego.CategoriaId), "La categoría seleccionada no existe o ya no está disponible.");

            if (!ModelState.IsValid)
            {
                juegoBD.Titulo = juego.Titulo;
                juegoBD.Precio = juego.Precio;
                juegoBD.CategoriaId = juego.CategoriaId;
                juegoBD.Descripcion = juego.Descripcion;
                juegoBD.EnPromocion = juego.EnPromocion;
                juegoBD.PrecioPromocion = juego.PrecioPromocion;
                juegoBD.EdadMinima = juego.EdadMinima;
                return View(juegoBD);
            }

            juegoBD.Titulo = juego.Titulo;
            juegoBD.Precio = juego.Precio;
            juegoBD.CategoriaId = juego.CategoriaId;
            juegoBD.Descripcion = juego.Descripcion;
            juegoBD.EnPromocion = juego.EnPromocion;
            juegoBD.PrecioPromocion = juego.EnPromocion ? juego.PrecioPromocion : null;
            juegoBD.EdadMinima = juego.EdadMinima;
            

            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(juegoBD.Imagen))
                {
                    var rutaAnterior = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        juegoBD.Imagen.TrimStart('/')
                    );

                    if (System.IO.File.Exists(rutaAnterior))
                        System.IO.File.Delete(rutaAnterior);
                }

                var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(archivoImagen.FileName)}";
                var rutaNueva = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "imagenes",
                    nombreArchivo
                );

                using (var stream = new FileStream(rutaNueva, FileMode.Create))
                {
                    await archivoImagen.CopyToAsync(stream);
                }

                juegoBD.Imagen = "/imagenes/" + nombreArchivo;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AdminAuthorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juego = await _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (juego == null)
            {
                return NotFound();
            }

            return View(juego);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var juego = await _context.VideoJuegos.FindAsync(id);
            if (juego != null)
            {
                _context.VideoJuegos.Remove(juego);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCategoriasAsync(int? categoriaSeleccionada = null)
        {
            var categorias = await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Id)
                .ToListAsync();

            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaSeleccionada);
        }

        private Task<bool> CategoriaExisteAsync(int categoriaId)
        {
            return _context.Categorias.AnyAsync(c => c.Id == categoriaId);
        }

        private List<CartSessionItem> ObtenerCarritoSession()
        {
            var json = HttpContext.Session.GetString(CarritoSessionKey);
            if (string.IsNullOrWhiteSpace(json))
                return new List<CartSessionItem>();

            try
            {
                return JsonSerializer.Deserialize<List<CartSessionItem>>(json) ?? new List<CartSessionItem>();
            }
            catch
            {
                return new List<CartSessionItem>();
            }
        }

        private void GuardarCarritoSession(List<CartSessionItem> carrito)
        {
            HttpContext.Session.SetString(CarritoSessionKey, JsonSerializer.Serialize(carrito));
        }

        private async Task<CarritoViewModel> ConstruirCarritoAsync()
        {
            var carritoSession = ObtenerCarritoSession();
            if (!carritoSession.Any())
                return new CarritoViewModel();

            var ids = carritoSession.Select(x => x.VideoJuegoId).Distinct().ToList();
            var juegos = await _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .Where(v => ids.Contains(v.Id))
                .ToListAsync();

            var items = carritoSession
                .Join(
                    juegos,
                    session => session.VideoJuegoId,
                    juego => juego.Id,
                    (session, juego) => new CarritoItemViewModel
                    {
                        VideoJuegoId = juego.Id,
                        Titulo = juego.Titulo,
                        Imagen = juego.Imagen,
                        Categoria = juego.Categoria != null ? juego.Categoria.Nombre : string.Empty,
                        Cantidad = session.Cantidad,
                        PrecioUnitario = juego.EnPromocion && juego.PrecioPromocion.HasValue
                            ? juego.PrecioPromocion.Value
                            : juego.Precio
                    })
                .OrderBy(x => x.Titulo)
                .ToList();

            return new CarritoViewModel
            {
                Items = items
            };
        }

        private static string LimpiarTarjeta(string? numeroTarjeta)
        {
            if (string.IsNullOrWhiteSpace(numeroTarjeta))
                return string.Empty;

            return new string(numeroTarjeta.Where(char.IsDigit).ToArray());
        }

        private static bool FormatoTarjetaValido(string numeroTarjeta)
        {
            return !string.IsNullOrWhiteSpace(numeroTarjeta) &&
                   numeroTarjeta.Length >= 13 &&
                   numeroTarjeta.Length <= 19 &&
                   numeroTarjeta.All(char.IsDigit);
        }

        private static bool ExpiracionPareceValida(string expMonth, string expYear)
        {
            if (!int.TryParse(expMonth, out var mes) || !int.TryParse(expYear, out var anioDosDigitos))
                return false;

            if (mes < 1 || mes > 12)
                return false;

            var anio = 2000 + anioDosDigitos;
            var ultimoDiaMes = DateTime.DaysInMonth(anio, mes);
            var fechaExp = new DateTime(anio, mes, ultimoDiaMes, 23, 59, 59);
            return fechaExp >= DateTime.Now.Date;
        }

        private async Task<(bool aprobado, string codigoTransaccion, string mensajeError)> ProcesarPagoSandboxConTarjetaAsync(
            string cardNumber,
            string expMonth,
            string expYear,
            string cvv,
            decimal total,
            string descripcion)
        {
            try
            {
                var baseUrl = _configuration["PovySandbox:BaseUrl"] ?? "https://backend-povy.onrender.com";
                var merchantName = _configuration["PovySandbox:MerchantName"] ?? "GameStore Sandbox";
                var currency = _configuration["PovySandbox:Currency"] ?? "USD";

                var payload = new
                {
                    cardNumber,
                    expMonth,
                    expYear,
                    cvv,
                    amount = total,
                    currency,
                    description = descripcion,
                    merchantName
                };

                var client = _httpClientFactory.CreateClient();
                using var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                using var response = await client.PostAsync($"{baseUrl.TrimEnd('/')}/api/payments/card", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return (false, string.Empty, $"El sandbox de pago devolvio un error: {body}");

                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                var codigoTransaccion =
                    ObtenerStringJson(root, "transactionId") ??
                    ObtenerStringJson(root, "paymentId") ??
                    ObtenerStringJson(root, "id") ??
                    ObtenerStringJson(root, "code") ??
                    Guid.NewGuid().ToString("N");

                var aprobado =
                    ObtenerBoolJson(root, "approved") ??
                    ObtenerBoolJson(root, "success") ??
                    EstadoEsExitoso(ObtenerStringJson(root, "status")) ??
                    true;

                if (!aprobado)
                {
                    var mensaje = ObtenerStringJson(root, "message") ??
                                  ObtenerStringJson(root, "error") ??
                                  "La compra no fue aprobada por el sandbox de pago.";
                    return (false, codigoTransaccion, mensaje);
                }

                return (true, codigoTransaccion, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, $"No fue posible conectar con Povy Sandbox: {ex.Message}");
            }
        }

        private static string? ObtenerStringJson(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var value) && value.ValueKind != JsonValueKind.Null)
                return value.ToString();

            return null;
        }

        private static bool? ObtenerBoolJson(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var value))
                return null;

            if (value.ValueKind == JsonValueKind.True)
                return true;

            if (value.ValueKind == JsonValueKind.False)
                return false;

            if (value.ValueKind == JsonValueKind.String && bool.TryParse(value.GetString(), out var result))
                return result;

            return null;
        }

        private static bool? EstadoEsExitoso(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;

            return status.Equals("approved", StringComparison.OrdinalIgnoreCase) ||
                   status.Equals("success", StringComparison.OrdinalIgnoreCase) ||
                   status.Equals("succeeded", StringComparison.OrdinalIgnoreCase) ||
                   status.Equals("completed", StringComparison.OrdinalIgnoreCase);
        }

        [AdminAuthorize]
        public async Task<IActionResult> GestionCategorias()
        {
            var model = await CrearGestionCategoriasViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<IActionResult> CrearCategoria(GestionCategoriasViewModel model)
        {
            model.NuevaCategoriaNombre = model.NuevaCategoriaNombre?.Trim() ?? string.Empty;

            if (await _context.Categorias.AnyAsync(c => c.Nombre == model.NuevaCategoriaNombre))
                ModelState.AddModelError(nameof(model.NuevaCategoriaNombre), "Ya existe una categoria con ese nombre.");

            if (!ModelState.IsValid)
            {
                var vm = await CrearGestionCategoriasViewModelAsync(model.NuevaCategoriaNombre);
                return View("GestionCategorias", vm);
            }

            _context.Categorias.Add(new Categoria { Nombre = model.NuevaCategoriaNombre });
            await _context.SaveChangesAsync();
            TempData["CategoriaMensaje"] = "Categoria creada correctamente.";
            return RedirectToAction(nameof(GestionCategorias));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<IActionResult> EditarCategoria(int id, string nombre)
        {
            nombre = nombre?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                TempData["CategoriaError"] = "El nombre de la categoria no puede estar vacio.";
                return RedirectToAction(nameof(GestionCategorias));
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();

            var existe = await _context.Categorias.AnyAsync(c => c.Id != id && c.Nombre == nombre);
            if (existe)
            {
                TempData["CategoriaError"] = "Ya existe otra categoria con ese nombre.";
                return RedirectToAction(nameof(GestionCategorias));
            }

            categoria.Nombre = nombre;
            await _context.SaveChangesAsync();
            TempData["CategoriaMensaje"] = "Categoria actualizada correctamente.";
            return RedirectToAction(nameof(GestionCategorias));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.VideoJuegos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            if (categoria.VideoJuegos.Any())
            {
                TempData["CategoriaError"] = "No puedes eliminar una categoria que todavia tiene videojuegos asociados.";
                return RedirectToAction(nameof(GestionCategorias));
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            TempData["CategoriaMensaje"] = "Categoria eliminada correctamente.";
            return RedirectToAction(nameof(GestionCategorias));
        }

        private async Task<GestionCategoriasViewModel> CrearGestionCategoriasViewModelAsync(string? nuevaCategoriaNombre = null)
        {
            return new GestionCategoriasViewModel
            {
                NuevaCategoriaNombre = nuevaCategoriaNombre ?? string.Empty,
                Categorias = await _context.Categorias
                    .AsNoTracking()
                    .Include(c => c.VideoJuegos)
                    .OrderBy(c => c.Nombre)
                    .ThenBy(c => c.Id)
                    .ToListAsync()
            };
        }
    }
}
