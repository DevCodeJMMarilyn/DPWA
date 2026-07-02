namespace API_Envios.Models;

public class HistorialEnvio
{
    public int Id { get; set; }
    public int EnvioId { get; set; }
    public int? UsuarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public EstadoEnvio Estado { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
}
