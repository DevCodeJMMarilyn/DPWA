using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appEnvios.Models
{
    public class Envio
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }
        
        public int DestinatarioId { get; set; }
        [ForeignKey("DestinatarioId")]
        public Destinatario Destinatario { get; set; }
        
        public DateTime FechaEnvio { get; set; } = DateTime.Now;
        [Required]
        public DateTime FechaEntrega { get; set; } = DateTime.Now;
        [Required]
        
        public int EstadoEnvioId { get; set; }
        [ForeignKey("EstadoEnvioId")]

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Costo { get; set; }
        public EstadoEnvio EstadoEnvio { get; set; }

    }
}
