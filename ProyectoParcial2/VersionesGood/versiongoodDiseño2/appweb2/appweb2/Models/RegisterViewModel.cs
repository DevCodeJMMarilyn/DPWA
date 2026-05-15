using System.ComponentModel.DataAnnotations;

namespace appweb2.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(103)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(153)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
