namespace API_Envios.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public bool Activo { get; set; } = true;
    public string? Departamento { get; set; }
    public string? Distrito { get; set; }
    public string? DireccionCercana { get; set; }
    public int? EmpresaId { get; set; }
    public int? PilotoId { get; set; }
}
