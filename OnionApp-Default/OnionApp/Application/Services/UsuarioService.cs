using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<object>> ObtenerTodos()
    {
        var usuarios = await _usuarioRepository.ObtenerTodos();

        return usuarios.Select(u => new
        {
            u.Id,
            u.Nombre,
            u.Email,
            Rol = u.Rol.ToString(),
            u.Activo,
            u.Departamento,
            u.Distrito,
            u.DireccionCercana
        }).Cast<object>().ToList();
    }

    public async Task<string> Crear(UsuarioDTO dto)
    {
        var existe = await _usuarioRepository.ObtenerPorEmail(dto.Email);

        if (existe != null)
        {
            return "Ya existe un usuario con ese email.";
        }

        if (dto.Rol == RolUsuario.Admin)
        {
            if (string.IsNullOrWhiteSpace(dto.Departamento) ||
                string.IsNullOrWhiteSpace(dto.Distrito))
            {
                return "Para crear un administrador debes indicar departamento y distrito.";
            }
        }

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = dto.Rol,
            Departamento = dto.Departamento,
            Distrito = dto.Distrito,
            DireccionCercana = dto.DireccionCercana,
            Activo = true
        };

        await _usuarioRepository.Crear(usuario);
        await _usuarioRepository.Guardar();

        return "Usuario creado correctamente.";
    }

    public async Task<string> Editar(int id, UsuarioDTO dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorId(id);

        if (usuario == null)
        {
            return "Usuario no encontrado.";
        }

        if (dto.Rol == RolUsuario.Admin)
        {
            if (string.IsNullOrWhiteSpace(dto.Departamento) ||
                string.IsNullOrWhiteSpace(dto.Distrito))
            {
                return "Para editar un administrador debes indicar departamento y distrito.";
            }
        }

        usuario.Nombre = dto.Nombre;
        usuario.Email = dto.Email;
        usuario.Rol = dto.Rol;
        usuario.Departamento = dto.Departamento;
        usuario.Distrito = dto.Distrito;
        usuario.DireccionCercana = dto.DireccionCercana;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _usuarioRepository.Actualizar(usuario);
        await _usuarioRepository.Guardar();

        return "Usuario actualizado correctamente.";
    }

    public async Task<string> Desactivar(int id)
    {
        var usuario = await _usuarioRepository.ObtenerPorId(id);

        if (usuario == null)
        {
            return "Usuario no encontrado.";
        }

        usuario.Activo = false;

        await _usuarioRepository.Actualizar(usuario);
        await _usuarioRepository.Guardar();

        return "Usuario desactivado correctamente.";
    }
}