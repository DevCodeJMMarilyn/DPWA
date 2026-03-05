using System.ComponentModel.DataAnnotations;

namespace appMishiEnvios.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Telefono { get; set; }

        public string Correo { get; set; }
    }
}
