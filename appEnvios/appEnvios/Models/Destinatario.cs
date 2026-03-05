using System.ComponentModel.DataAnnotations;

namespace appEnvios.Models
{
    public class Destinatario
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
        [StringLength(200)]
        public string direccion { get; set; }
        [Required]
        [StringLength(150)]
        public string ciudad { get; set; }
        [Required]
        [StringLength(150)]
        public string pais { get; set; }

        public ICollection<Envio> Envios { get; set; }
    }
}
