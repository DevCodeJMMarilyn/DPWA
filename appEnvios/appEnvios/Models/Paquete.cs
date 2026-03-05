using System.ComponentModel.DataAnnotations;

namespace appEnvios.Models
{
    public class Paquete
    {
        [Key]
        public int Id { get; set; }
        public int EnvioId { get; set; }
        public Envio Envio { get; set; }
        public double Peso { get; set; }

        public ICollection<Envio> Envios { get; set; }
    }
}
