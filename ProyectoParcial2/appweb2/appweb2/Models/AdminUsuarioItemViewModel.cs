using System;

namespace appweb2.Models
{
    public class AdminUsuarioItemViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public bool EsUsuarioActual { get; set; }
    }
}
