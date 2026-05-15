using System.ComponentModel.DataAnnotations;

namespace appweb2.Models
{
    public class GestionCuentaViewModel
    {
        [Required]
        [StringLength(103)]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Correo")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Contrasena actual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contrasena")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contrasena")]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
