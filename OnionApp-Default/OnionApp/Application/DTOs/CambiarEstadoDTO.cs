using Domain.Enums;

namespace Application.DTOs;

public class CambiarEstadoDTO
{
    public int EnvioId { get; set; }

    public EstadoEnvio Estado { get; set; }

    public string? Observaciones { get; set; }
}