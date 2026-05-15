namespace appweb2.Models
{
    public class ReporteVentaDetalleItemViewModel
    {
        public int CompraId { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Juego { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string CodigoTransaccion { get; set; } = string.Empty;
    }
}
