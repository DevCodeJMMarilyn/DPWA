namespace appweb2.Models
{
    public class ReporteUsuariosPdfViewModel
    {
        public DateTime FechaGeneracion { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string Busqueda { get; set; } = string.Empty;
        public string RolFiltro { get; set; } = string.Empty;
        public string? DesdeTexto { get; set; }
        public string? HastaTexto { get; set; }
        public int TotalUsuarios { get; set; }
        public List<ReporteUsuariosRolResumenViewModel> ResumenRoles { get; set; } = new();
        public List<AdminUsuarioItemViewModel> Usuarios { get; set; } = new();
    }

    public class ReporteUsuariosRolResumenViewModel
    {
        public string Rol { get; set; } = string.Empty;
        public int Total { get; set; }
    }
}
