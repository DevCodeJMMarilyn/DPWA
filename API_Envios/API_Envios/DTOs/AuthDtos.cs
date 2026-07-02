using API_Envios.Models;

namespace API_Envios.DTOs;

public record LoginRequest(string Correo, string Password);

public record LoginResponse(
    string Token,
    int UsuarioId,
    string Nombre,
    RolUsuario Rol,
    int? EmpresaId,
    int? PilotoId);
