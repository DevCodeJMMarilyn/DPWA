using Domain.Enums;

namespace Domain.Entities;

public class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; }

    public bool Activo { get; set; } = true;

    public string? Departamento { get; set; }

    public string? Distrito { get; set; }

    public string? DireccionCercana { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public EmpresaCliente? EmpresaCliente { get; set; }

    public Piloto? Piloto { get; set; }
}