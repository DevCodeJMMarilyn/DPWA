using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace appweb2.Models
{
    public class AdminUsuariosViewModel
    {
        public List<AdminUsuarioItemViewModel> Usuarios { get; set; } = new();
        public List<SelectListItem> RolesDisponibles { get; set; } = new();
    }
}
