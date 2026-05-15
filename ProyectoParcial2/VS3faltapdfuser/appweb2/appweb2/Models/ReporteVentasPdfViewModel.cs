namespace appweb2.Models
{
    public class ReporteVentasPdfViewModel
    {
        public DateTime FechaGeneracion { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string? DesdeTexto { get; set; }
        public string? HastaTexto { get; set; }
        public string? Cliente { get; set; }
        public string? Videojuego { get; set; }
        public int TotalTransacciones { get; set; }
        public int TotalUnidadesVendidas { get; set; }
        public decimal IngresoTotal { get; set; }
        public List<ReporteVentaDetalleItemViewModel> Ventas { get; set; } = new();
    }
}
