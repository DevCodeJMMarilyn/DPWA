using System;

namespace appweb2.Models
{
    public class VentaViewModel
    {
        public int idCompra { get; set; }
        public int UsuarioId { get; set; }
        public int? VideoJuegoID { get; set; }
        public string? VideoJuegoTitulo { get; set; }
        public string VideoJuegosId { get; set; }
        public int cantidad { get; set; }
        public decimal total { get; set; }
        public string estadoCompra { get; set; }
        public DateTime fechaHoraTransaccion { get; set; }
        public string codigoTransaccion { get; set; }
    }
}
