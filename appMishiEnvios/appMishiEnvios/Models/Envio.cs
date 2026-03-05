using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appMishiEnvios.Models
{
    public class Envio
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaEnvio { get; set; }

        public int DestinatarioId { get; set; }

        [ForeignKey("DestinatarioId")]
        public Destinatario Destinatario { get; set; }

        public int EstadoEnvioId { get; set; }

        [ForeignKey("EstadoEnvioId")]
        public EstadoEnvio EstadoEnvio { get; set; }
    }
}
