using API_Envios.Models;

namespace API_Envios.DTOs;

public record UsuarioCreateRequest(
    string Nombre,
    string Correo,
    string Password,
    RolUsuario Rol,
    string? Departamento,
    string? Distrito,
    string? DireccionCercana,
    int? EmpresaId,
    int? PilotoId);

public record UsuarioUpdateRequest(
    string Nombre,
    string Correo,
    string? Password,
    bool Activo,
    string? Departamento,
    string? Distrito,
    string? DireccionCercana,
    int? EmpresaId,
    int? PilotoId);

public record UsuarioResponse(
    int Id,
    string Nombre,
    string Correo,
    RolUsuario Rol,
    bool Activo,
    string? Departamento,
    string? Distrito,
    string? DireccionCercana,
    int? EmpresaId,
    int? PilotoId);
