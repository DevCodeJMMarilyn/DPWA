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
using System.Net.Http.Headers;

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

        public async Task<IActionResult> Index(int pagina = 1)
        {
            var query = _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .OrderBy(v => v.Titulo);

            var model = await CrearCatalogoViewModelAsync(
                query,
                pagina,
                nameof(Index),
                "Videojuegos disponibles",
                mostrarBanner: true);

            return View(model);
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

        public async Task<IActionResult> PorCategoria(int? id, int pagina = 1)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(Categorias));

            var query = _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .Where(v => v.CategoriaId == id.Value)
                .OrderBy(v => v.Titulo);

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id.Value);

            var nombreCategoria = categoria != null ? categoria.Nombre : string.Empty;
            var encabezado = string.IsNullOrWhiteSpace(nombreCategoria)
                ? "Videojuegos disponibles"
                : $"Categoria: {nombreCategoria}";

            var model = await CrearCatalogoViewModelAsync(
                query,
                pagina,
                nameof(PorCategoria),
                encabezado,
                categoriaId: id.Value,
                mostrarBanner: true);

            return View("Index", model);
        }

        public async Task<IActionResult> NuevosJuegos(int pagina = 1)
        {
            var query = _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .OrderByDescending(v => v.FechaRegistro)
                .ThenBy(v => v.Titulo);

            var model = await CrearCatalogoViewModelAsync(
                query,
                pagina,
                nameof(NuevosJuegos),
                "Nuevos videojuegos",
                mostrarBanner: true);

            return View("Index", model);
        }

        public async Task<IActionResult> Promociones(int pagina = 1)
        {
            var query = _context.VideoJuegos
                .AsNoTracking()
                .Include(v => v.Categoria)
                .Where(v => v.EnPromocion)
                .OrderBy(v => v.Titulo);

            var model = await CrearCatalogoViewModelAsync(
                query,
                pagina,
                nameof(Promociones),
                "Videojuegos en promocion",
                mostrarBanner: true);

            return View("Index", model);
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

            carrito.MetodoPago = "PayPal";
            carrito.PayPalOrderId = model.PayPalOrderId?.Trim() ?? string.Empty;
            carrito.PayPalCaptureId = model.PayPalCaptureId?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(carrito.PayPalOrderId) || string.IsNullOrWhiteSpace(carrito.PayPalCaptureId))
            {
                TempData["CarritoError"] = "Debes completar el pago con PayPal Sandbox antes de registrar la compra.";
                return RedirectToAction(nameof(Carrito));
            }

            var validacionPayPal = await ValidarPagoPayPalSandboxAsync(
                carrito.PayPalOrderId,
                carrito.PayPalCaptureId,
                carrito.Total);
            if (!validacionPayPal.aprobado)
            {
                TempData["CarritoError"] = validacionPayPal.mensajeError;
                return RedirectToAction(nameof(Carrito));
            }

            var codigoTransaccion = !string.IsNullOrWhiteSpace(validacionPayPal.codigoTransaccion)
                ? validacionPayPal.codigoTransaccion
                : carrito.PayPalCaptureId;

            if (!ModelState.IsValid)
                return View("Carrito", carrito);

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
                        codigoTransaccion = codigoTransaccion,
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
            TempData["CarritoMensaje"] = $"Compra aprobada con PayPal Sandbox. Codigo de transaccion: {codigoTransaccion}";
            return RedirectToAction("MisCompras", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearOrdenPayPal()
        {
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if (!usuarioId.HasValue)
                return Unauthorized(new { error = "Tu sesion expiro. Inicia sesion nuevamente antes de pagar." });

            var carrito = await ConstruirCarritoAsync();
            if (!carrito.Items.Any())
                return BadRequest(new { error = "Tu carrito esta vacio." });

            var token = await ObtenerAccessTokenPayPalSandboxAsync();
            if (!token.aprobado)
                return BadRequest(new { error = token.mensajeError });

            var currency = _configuration["PayPalSandbox:Currency"] ?? "USD";
            var appName = _configuration["PayPalSandbox:AppName"] ?? "MishiGamesProject";
            var baseUrl = ObtenerPayPalBaseUrl();
            var client = _httpClientFactory.CreateClient();

            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        description = $"Compra en {appName}",
                        amount = new
                        {
                            currency_code = currency,
                            value = carrito.Total.ToString("0.00", CultureInfo.InvariantCulture)
                        }
                    }
                },
                application_context = new
                {
                    brand_name = appName,
                    shipping_preference = "NO_SHIPPING",
                    user_action = "PAY_NOW"
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v2/checkout/orders");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.codigoTransaccion);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return BadRequest(new { error = $"PayPal Sandbox no pudo crear la orden: {body}" });

            using var document = JsonDocument.Parse(body);
            var orderId = ObtenerStringJson(document.RootElement, "id");
            if (string.IsNullOrWhiteSpace(orderId))
                return BadRequest(new { error = "PayPal Sandbox no devolvio el id de la orden." });

            return Json(new { id = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapturarOrdenPayPal([FromBody] PayPalCaptureRequest model)
        {
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if (!usuarioId.HasValue)
                return Unauthorized(new { error = "Tu sesion expiro. Inicia sesion nuevamente antes de pagar." });

            var orderId = model.OrderId?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(orderId))
                return BadRequest(new { error = "PayPal no devolvio una orden valida." });

            var carrito = await ConstruirCarritoAsync();
            if (!carrito.Items.Any())
                return BadRequest(new { error = "Tu carrito esta vacio." });

            var token = await ObtenerAccessTokenPayPalSandboxAsync();
            if (!token.aprobado)
                return BadRequest(new { error = token.mensajeError });

            var baseUrl = ObtenerPayPalBaseUrl();
            var client = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v2/checkout/orders/{orderId}/capture");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.codigoTransaccion);
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return BadRequest(new { error = $"PayPal Sandbox no pudo capturar la orden: {body}" });

            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;
            var status = ObtenerStringJson(root, "status");
            if (!string.Equals(status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "La orden de PayPal Sandbox no quedo completada." });

            var captureId = ObtenerCaptureIdPayPal(root);
            if (string.IsNullOrWhiteSpace(captureId))
                return BadRequest(new { error = "PayPal Sandbox no devolvio el codigo de captura." });

            var currency = _configuration["PayPalSandbox:Currency"] ?? "USD";
            if (!PagoPayPalCoincide(root, captureId, carrito.Total, currency))
                return BadRequest(new { error = "El monto de PayPal Sandbox no coincide con el carrito." });

            try
            {
                await RegistrarCompraAsync(usuarioId.Value, carrito, captureId);
            }
            catch
            {
                return BadRequest(new { error = "El pago fue aprobado, pero ocurrio un problema al registrar la compra en la base de datos." });
            }

            GuardarCarritoSession(new List<CartSessionItem>());
            TempData["CarritoMensaje"] = $"Compra aprobada con PayPal Sandbox. Codigo de transaccion: {captureId}";

            return Json(new
            {
                redirectUrl = Url.Action("MisCompras", "Account")
            });
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
                Items = items,
                PayPalClientId = _configuration["PayPalSandbox:ClientId"] ?? string.Empty,
                PayPalCurrency = _configuration["PayPalSandbox:Currency"] ?? "USD",
                PayPalAppName = _configuration["PayPalSandbox:AppName"] ?? "MishiGamesProject"
            };
        }

        private async Task<(bool aprobado, string codigoTransaccion, string mensajeError)> ValidarPagoPayPalSandboxAsync(
            string orderId,
            string captureId,
            decimal total)
        {
            var currency = _configuration["PayPalSandbox:Currency"] ?? "USD";

            var token = await ObtenerAccessTokenPayPalSandboxAsync();
            if (!token.aprobado)
                return token;

            var baseUrl = ObtenerPayPalBaseUrl();
            var client = _httpClientFactory.CreateClient();

            using var orderRequest = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/v2/checkout/orders/{orderId}");
            orderRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.codigoTransaccion);

            using var orderResponse = await client.SendAsync(orderRequest);
            var orderBody = await orderResponse.Content.ReadAsStringAsync();
            if (!orderResponse.IsSuccessStatusCode)
                return (false, string.Empty, $"PayPal Sandbox no pudo validar la orden: {orderBody}");

            using var orderDocument = JsonDocument.Parse(orderBody);
            var root = orderDocument.RootElement;
            var status = ObtenerStringJson(root, "status");
            if (!string.Equals(status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                return (false, string.Empty, "La orden de PayPal Sandbox no esta completada.");

            if (!PagoPayPalCoincide(root, captureId, total, currency))
                return (false, string.Empty, "El monto o la captura de PayPal Sandbox no coincide con el carrito.");

            return (true, captureId, string.Empty);
        }

        private async Task<(bool aprobado, string codigoTransaccion, string mensajeError)> ObtenerAccessTokenPayPalSandboxAsync()
        {
            var clientId = _configuration["PayPalSandbox:ClientId"] ?? string.Empty;
            var clientSecret = _configuration["PayPalSandbox:ClientSecret"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(clientId))
                return (false, string.Empty, "Falta configurar el Client ID de PayPal Sandbox.");

            if (string.IsNullOrWhiteSpace(clientSecret))
                return (false, string.Empty, "Falta configurar el Client Secret de PayPal Sandbox.");

            var baseUrl = ObtenerPayPalBaseUrl();
            var client = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v1/oauth2/token");
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return (false, string.Empty, $"PayPal Sandbox no autorizo la operacion: {body}");

            using var document = JsonDocument.Parse(body);
            var accessToken = ObtenerStringJson(document.RootElement, "access_token");
            if (string.IsNullOrWhiteSpace(accessToken))
                return (false, string.Empty, "PayPal Sandbox no devolvio un token de acceso.");

            return (true, accessToken, string.Empty);
        }

        private string ObtenerPayPalBaseUrl()
        {
            return (_configuration["PayPalSandbox:BaseUrl"] ?? "https://api-m.sandbox.paypal.com").TrimEnd('/');
        }

        private async Task RegistrarCompraAsync(int usuarioId, CarritoViewModel carrito, string codigoTransaccion)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in carrito.Items)
                {
                    var compra = new Compra
                    {
                        FechaCompra = DateTime.Now,
                        UsuarioID = usuarioId,
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
                        codigoTransaccion = codigoTransaccion,
                        idCompra = compra.Id
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static string? ObtenerCaptureIdPayPal(JsonElement root)
        {
            if (!root.TryGetProperty("purchase_units", out var purchaseUnits) || purchaseUnits.ValueKind != JsonValueKind.Array)
                return null;

            foreach (var purchaseUnit in purchaseUnits.EnumerateArray())
            {
                if (!purchaseUnit.TryGetProperty("payments", out var payments) ||
                    !payments.TryGetProperty("captures", out var captures) ||
                    captures.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var capture in captures.EnumerateArray())
                {
                    var status = ObtenerStringJson(capture, "status");
                    if (string.Equals(status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                        return ObtenerStringJson(capture, "id");
                }
            }

            return null;
        }

        private static bool PagoPayPalCoincide(JsonElement root, string captureId, decimal total, string currency)
        {
            if (!root.TryGetProperty("purchase_units", out var purchaseUnits) || purchaseUnits.ValueKind != JsonValueKind.Array)
                return false;

            foreach (var purchaseUnit in purchaseUnits.EnumerateArray())
            {
                if (!purchaseUnit.TryGetProperty("payments", out var payments) ||
                    !payments.TryGetProperty("captures", out var captures) ||
                    captures.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var capture in captures.EnumerateArray())
                {
                    var id = ObtenerStringJson(capture, "id");
                    var status = ObtenerStringJson(capture, "status");
                    if (!string.Equals(id, captureId, StringComparison.OrdinalIgnoreCase) ||
                        !string.Equals(status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!capture.TryGetProperty("amount", out var amount))
                        continue;

                    var value = ObtenerStringJson(amount, "value");
                    var captureCurrency = ObtenerStringJson(amount, "currency_code");
                    if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var captureTotal))
                        continue;

                    return string.Equals(captureCurrency, currency, StringComparison.OrdinalIgnoreCase) &&
                           captureTotal == Math.Round(total, 2);
                }
            }

            return false;
        }

        private static string? ObtenerStringJson(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var value) && value.ValueKind != JsonValueKind.Null)
                return value.ToString();

            return null;
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

        private async Task<CatalogoVideoJuegosViewModel> CrearCatalogoViewModelAsync(
            IOrderedQueryable<VideoJuego> query,
            int pagina,
            string accionActual,
            string encabezado,
            int? categoriaId = null,
            bool mostrarBanner = true)
        {
            const int tamanoPagina = 10;
            pagina = pagina < 1 ? 1 : pagina;

            var totalRegistros = await query.CountAsync();
            var totalPaginas = totalRegistros == 0 ? 1 : (int)Math.Ceiling(totalRegistros / (double)tamanoPagina);
            if (pagina > totalPaginas)
                pagina = totalPaginas;

            var juegos = await query
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new CatalogoVideoJuegosViewModel
            {
                Juegos = juegos,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                AccionActual = accionActual,
                CategoriaId = categoriaId,
                Encabezado = encabezado,
                MostrarBanner = mostrarBanner
            };
        }
    }
}
