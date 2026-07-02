namespace Application.DTOs;

public class EntregaDTO
{
    public int EnvioId { get; set; }

    public string FirmaCliente { get; set; } = string.Empty;

    public string Imagen1 { get; set; } = string.Empty;

    public string? Imagen2 { get; set; }

    public string? Observaciones { get; set; }
}