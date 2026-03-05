using System.ComponentModel.DataAnnotations;

namespace appMishiEnvios.Models
{
    public class Auditoria
    {
        [Key]
        public int Id { get; set; }

        public string Tabla { get; set; }

        public string Accion { get; set; }

        public string Usuario { get; set; }

        public DateTime Fecha { get; set; }

        public int RegistroId { get; set; }
    }
}

