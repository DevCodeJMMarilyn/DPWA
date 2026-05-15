using System.ComponentModel.DataAnnotations;

namespace appweb2.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(103)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(153)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] Password { get; set; } = Array.Empty<byte>();

        [Required]
        [StringLength(50)]
        public string Salt { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        public Role Role { get; set; } = null!;

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection <Compra> compras { get; set; } = new List<Compra>();
    }
}
