using appWeb2.Data;
using appWeb2.Filtros;
using appWeb2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace appWeb2.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [SessionAuthorize]
        public IActionResult Dashboard()
        {
            //var data = (from v in _context.VideoJuegos
            //            join c in _context.categorias
            //            on v.idcategoria equals c.idcategoria
            //            group v by c.categoria into g
            //            select new
            //            {
            //                Categoria = g.Key,
            //                Total = g.Count()
            //            }).ToList();
            //ViewBag.Categorias = data.Select(x=> x.Categoria).ToList();
            //ViewBag.Totales = data.Select(x => x.Total).ToList();
            return View();
        }

        //// GET: /Account/Index  → muestra el formulario de login
        public IActionResult Index()
        {
            return View("Login");
        }

        //// GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        ////codigo de clase grafica quemada
        ////public IActionResult ObtenerDatos(string categoria)
        ////{
        ////    var query = from v in _context.VideoJuegos
        ////                join c in _context.categorias
        ////                on v.idcategoria equals c.idcategoria
        ////                select new { c.categoria };
        ////    if (!string.IsNullOrEmpty(categoria))
        ////    {
        ////        query = query.Where(x => x.categoria == categoria);
        ////    }

        ////    var data = query
        ////        .GroupBy(x => x.categoria)
        ////        .Select(g => new
        ////        {
        ////            categoria = g.Key,
        ////            total = g.Count()
        ////        }).ToList();

        ////    return Json(data);
        ////}
        //Codigo de graficos sql 
        public IActionResult ObtenerDatos(int? idcategoria)
        {
            var query = from v in _context.VideoJuegos
                        join c in _context.categorias
                        on v.idcategoria equals c.idcategoria
                        select new { c.categoria, v.idcategoria };

            if (idcategoria.HasValue)
            {
                query = query.Where(x => x.idcategoria == idcategoria.Value);
            }

            var data = query
                .GroupBy(x => x.categoria)
                .Select(g => new
                {
                    categoria = g.Key,
                    total = g.Count()
                }).ToList();

            return Json(data);
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Usuarios
                .FirstOrDefault(u => u.correo == model.correo);

            if (user != null)
            {
                // Reconstruir el hash con el salt guardado + la password que escribió el usuario
                string saltedPassword = user.salt + model.password;

                using (SHA256 sha256 = SHA256.Create())
                {
                    //byte[] inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
                    byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);
                    
                    //Console.WriteLine("Salt DB: " + user.salt);
                    //Console.WriteLine("Password input: " + model.password);
                    //Console.WriteLine("Salted: " + (user.salt + model.password));

                    //Console.WriteLine("Hash generado: " + Convert.ToBase64String(hashBytes));
                    //Console.WriteLine("Hash DB: " + Convert.ToBase64String(user.password));
                    // Comparar el hash calculado con el guardado en BD
                    if (hashBytes.SequenceEqual(user.password))
                    {
                        HttpContext.Session.SetString("usuario", user.nombre);
                        //HttpContext.Session.SetString("nombre", user.nombre);

                        return RedirectToAction("Dashboard", "Account");
                    }
                }
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
