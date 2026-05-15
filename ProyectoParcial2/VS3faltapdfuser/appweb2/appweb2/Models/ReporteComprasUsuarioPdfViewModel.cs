namespace appweb2.Models
{
    public class ReporteComprasUsuarioPdfViewModel
    {
        public DateTime FechaGeneracion { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        public string? DesdeTexto { get; set; }
        public string? HastaTexto { get; set; }
        public string? Juego { get; set; }
        public int TotalCompras { get; set; }
        public int TotalUnidades { get; set; }
        public decimal TotalInvertido { get; set; }
        public List<CompraUsuarioViewModel> Compras { get; set; } = new();
    }
}
