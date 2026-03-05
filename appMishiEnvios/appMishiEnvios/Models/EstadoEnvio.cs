using System.ComponentModel.DataAnnotations;

namespace appMishiEnvios.Models
{
    public class EstadoEnvio
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
