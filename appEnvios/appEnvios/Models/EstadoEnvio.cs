using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appEnvios.Models
{
    public class EstadoEnvio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreEstado { get; set; }

        [Required]
        [StringLength(500)]
        public string descripcion { get; set; }

        public ICollection<Envio> Envios { get; set; }

    }
}
