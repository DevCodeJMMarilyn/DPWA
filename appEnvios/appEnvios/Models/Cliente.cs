using System.ComponentModel.DataAnnotations;

namespace appEnvios.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string telefono { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string correo { get; set; }

        [Required]
        [StringLength(255)]
        public string direccion { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

      
    }
}
