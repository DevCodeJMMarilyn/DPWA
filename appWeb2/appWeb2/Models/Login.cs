using System.ComponentModel.DataAnnotations;

namespace appWeb2.Models
{
    public class Login
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        public string correo { get; set; } = string.Empty;

        // password como string (lo que escribe el usuario)
        // El hash se calcula en el controlador antes de comparar con BD
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string password { get; set; } = string.Empty;
    }
}
