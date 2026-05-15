using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace appweb2.Models
{
    public class AdminUsuariosViewModel
    {
        public string Busqueda { get; set; } = string.Empty;
        public int? RoleIdFiltro { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public List<AdminUsuarioItemViewModel> Usuarios { get; set; } = new();
        public List<SelectListItem> RolesDisponibles { get; set; } = new();
    }
}
