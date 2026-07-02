namespace API_Envios.Models;

public class Envio
{
    public int Id { get; set; }
    public string CodigoRastreo { get; set; } = string.Empty;
    public int EmpresaId { get; set; }
    public int DestinatarioId { get; set; }
    public int? PilotoId { get; set; }
    public string DescripcionPedido { get; set; } = string.Empty;
    public decimal PesoLibras { get; set; }
    public EstadoEnvio Estado { get; set; } = EstadoEnvio.Recolectado;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEntrega { get; set; }
    public string? FirmaRecibido { get; set; }
    public List<string> ImagenesEntrega { get; set; } = [];
    public List<HistorialEnvio> Historial { get; set; } = [];
}
