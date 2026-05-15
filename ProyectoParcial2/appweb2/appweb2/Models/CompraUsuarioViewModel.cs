namespace appweb2.Models
{
    public class CompraUsuarioViewModel
    {
        public int Numero { get; set; }
        public int CompraId { get; set; }
        public string Juego { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string CodigoTransaccion { get; set; } = string.Empty;
    }
}
