using Domain.Enums;

namespace Application.DTOs;

public class UsuarioDTO
{
    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; }

    public string? Departamento { get; set; }

    public string? Distrito { get; set; }

    public string? DireccionCercana { get; set; }
}