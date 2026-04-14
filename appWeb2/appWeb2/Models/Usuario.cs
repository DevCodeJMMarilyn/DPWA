using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appWeb2.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string correo { get; set; } = string.Empty;

        // Guardado como VARBINARY(64) en SQL Server → byte[] en C#
        public byte[] password { get; set; } = Array.Empty<byte>();

        [Required]
        [StringLength(50)]
        public string salt { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
