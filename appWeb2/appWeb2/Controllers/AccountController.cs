using appWeb2.Data;
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

        // GET: /Account/Index  → muestra el formulario de login
        public IActionResult Index()
        {
            return View("Login");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
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
                    byte[] inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);

                    // Comparar el hash calculado con el guardado en BD
                    if (hashBytes.SequenceEqual(user.password))
                    {
                        HttpContext.Session.SetString("usuario", user.nombre);
                        HttpContext.Session.SetInt32("usuarioId", user.Id);
                        return RedirectToAction("Index", "Home");
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
