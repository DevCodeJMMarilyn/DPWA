using System.ComponentModel.DataAnnotations;
using System.Data;

namespace appWeb2.Models
{
    public class Usuario
    {
        [Key]
        [StringLength(100)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string correo { get; set; }

        [Required]
        [StringLength(255)]
        public string password { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;  
        
        public ICollection<Compra> Compras { get; set; }
    }
}
