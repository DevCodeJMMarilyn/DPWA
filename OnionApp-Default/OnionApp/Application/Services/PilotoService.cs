using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class PilotoService
{
    private readonly IPilotoRepository _pilotoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public PilotoService(IPilotoRepository pilotoRepository, IUsuarioRepository usuarioRepository)
    {
        _pilotoRepository = pilotoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<object>> ObtenerTodos()
    {
        var pilotos = await _pilotoRepository.ObtenerTodos();

        return MapearPilotos(pilotos);
    }

    public async Task<List<object>> ObtenerTodosPorRol(
        string rol,
        string? departamento,
        string? distrito)
    {
        var pilotos = await _pilotoRepository.ObtenerTodos();

        if (rol == "Admin")
        {
            pilotos = pilotos
                .Where(p =>
                    p.Departamento == departamento &&
                    p.Distrito == distrito)
                .ToList();
        }

        return MapearPilotos(pilotos);
    }

    public async Task<string> Crear(PilotoDTO dto)
    {
        var existe = await _usuarioRepository.ObtenerPorEmail(dto.Email);

        if (existe != null)
        {
            return "Ya existe un usuario con ese email.";
        }

        var usuario = new Usuario
        {
            Nombre = dto.NombreUsuario,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = RolUsuario.Piloto,
            Departamento = dto.Departamento,
            Distrito = dto.Distrito,
            Activo = true
        };

        var piloto = new Piloto
        {
            Usuario = usuario,
            Licencia = dto.Licencia,
            Telefono = dto.Telefono,
            Departamento = dto.Departamento,
            Distrito = dto.Distrito,
            Activo = true
        };

        await _pilotoRepository.Crear(piloto);
        await _pilotoRepository.Guardar();

        return "Piloto creado correctamente.";
    }

    public async Task<string> Editar(
        int id,
        PilotoDTO dto,
        string rol,
        string? departamento,
        string? distrito)
    {
        var piloto = await _pilotoRepository.ObtenerPorId(id);

        if (piloto == null)
        {
            return "Piloto no encontrado.";
        }

        if (rol == "Admin")
        {
            if (piloto.Departamento != departamento || piloto.Distrito != distrito)
            {
                return "No puedes editar pilotos fuera de tu zona.";
            }

            if (dto.Departamento != departamento || dto.Distrito != distrito)
            {
                return "No puedes mover el piloto fuera de tu zona.";
            }
        }

        piloto.Licencia = dto.Licencia;
        piloto.Telefono = dto.Telefono;
        piloto.Departamento = dto.Departamento;
        piloto.Distrito = dto.Distrito;

        piloto.Usuario.Nombre = dto.NombreUsuario;
        piloto.Usuario.Email = dto.Email;
        piloto.Usuario.Departamento = dto.Departamento;
        piloto.Usuario.Distrito = dto.Distrito;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            piloto.Usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _pilotoRepository.Actualizar(piloto);
        await _pilotoRepository.Guardar();

        return "Piloto actualizado correctamente.";
    }

    public async Task<string> DarDeBaja(
        int id,
        string rol,
        string? departamento,
        string? distrito)
    {
        var piloto = await _pilotoRepository.ObtenerPorId(id);

        if (piloto == null)
        {
            return "Piloto no encontrado.";
        }

        if (rol == "Admin")
        {
            if (piloto.Departamento != departamento || piloto.Distrito != distrito)
            {
                return "No puedes dar de baja pilotos fuera de tu zona.";
            }
        }

        piloto.Activo = false;
        piloto.Usuario.Activo = false;

        await _pilotoRepository.Actualizar(piloto);
        await _pilotoRepository.Guardar();

        return "Piloto dado de baja correctamente.";
    }

    private List<object> MapearPilotos(List<Piloto> pilotos)
    {
        return pilotos.Select(p => new
        {
            p.Id,
            p.Licencia,
            p.Telefono,
            p.Departamento,
            p.Distrito,
            p.Activo,
            Usuario = new
            {
                p.Usuario.Id,
                p.Usuario.Nombre,
                p.Usuario.Email
            }
        }).Cast<object>().ToList();
    }
}