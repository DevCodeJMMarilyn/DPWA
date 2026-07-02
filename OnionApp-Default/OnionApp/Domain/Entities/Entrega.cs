namespace Domain.Entities;

public class Entrega
{
    public int Id { get; set; }

    public int EnvioId { get; set; }

    public Envio Envio { get; set; } = null!;

    public string FirmaCliente { get; set; } = string.Empty;

    public string Imagen1 { get; set; } = string.Empty;

    public string? Imagen2 { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaEntrega { get; set; } = DateTime.Now;
}