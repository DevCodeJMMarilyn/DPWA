using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class DestinatarioService
{
    private readonly IDestinatarioRepository _destinatarioRepository;
    private readonly IEmpresaRepository _empresaRepository;

    public DestinatarioService(
        IDestinatarioRepository destinatarioRepository,
        IEmpresaRepository empresaRepository)
    {
        _destinatarioRepository = destinatarioRepository;
        _empresaRepository = empresaRepository;
    }

    public async Task<List<object>> ObtenerTodos()
    {
        var destinatarios = await _destinatarioRepository.ObtenerTodos();

        return MapearDestinatarios(destinatarios);
    }

    public async Task<List<object>> ObtenerTodosPorRol(
        int usuarioId,
        string rol,
        string? departamento,
        string? distrito)
    {
        var destinatarios = await _destinatarioRepository.ObtenerTodos();

        if (rol == "Admin")
        {
            destinatarios = destinatarios
                .Where(d =>
                    d.Departamento == departamento &&
                    d.Distrito == distrito)
                .ToList();
        }

        if (rol == "Empresa")
        {
            destinatarios = destinatarios
                .Where(d => d.EmpresaCliente.UsuarioId == usuarioId)
                .ToList();
        }

        return MapearDestinatarios(destinatarios);
    }

    public async Task<string> Crear(
        DestinatarioDTO dto,
        int usuarioId,
        string rol,
        string? departamento,
        string? distrito)
    {
        var empresa = await _empresaRepository.ObtenerPorId(dto.EmpresaClienteId);

        if (empresa == null || !empresa.Activa)
        {
            return "Empresa no encontrada o inactiva.";
        }

        if (rol == "Admin")
        {
            if (dto.Departamento != departamento || dto.Distrito != distrito)
            {
                return "No puedes crear destinatarios fuera de tu zona.";
            }
        }

        if (rol == "Empresa")
        {
            if (empresa.UsuarioId != usuarioId)
            {
                return "No puedes crear destinatarios para otra empresa.";
            }
        }

        var destinatario = new Destinatario
        {
            EmpresaClienteId = dto.EmpresaClienteId,
            Nombre = dto.Nombre,
            Telefono = dto.Telefono,
            Departamento = dto.Departamento,
            Distrito = dto.Distrito,
            Direccion = dto.Direccion,
            Referencia = dto.Referencia,
            Activo = true
        };

        await _destinatarioRepository.Crear(destinatario);
        await _destinatarioRepository.Guardar();

        return "Destinatario creado correctamente.";
    }

    public async Task<string> Editar(
        int id,
        DestinatarioDTO dto,
        int usuarioId,
        string rol,
        string? departamento,
        string? distrito)
    {
        var destinatario = await _destinatarioRepository.ObtenerPorId(id);

        if (destinatario == null)
        {
            return "Destinatario no encontrado.";
        }

        if (rol == "Admin")
        {
            if (destinatario.Departamento != departamento || destinatario.Distrito != distrito)
            {
                return "No puedes editar destinatarios fuera de tu zona.";
            }

            if (dto.Departamento != departamento || dto.Distrito != distrito)
            {
                return "No puedes mover el destinatario fuera de tu zona.";
            }
        }

        if (rol == "Empresa")
        {
            if (destinatario.EmpresaCliente.UsuarioId != usuarioId)
            {
                return "No puedes editar destinatarios de otra empresa.";
            }
        }

        destinatario.Nombre = dto.Nombre;
        destinatario.Telefono = dto.Telefono;
        destinatario.Departamento = dto.Departamento;
        destinatario.Distrito = dto.Distrito;
        destinatario.Direccion = dto.Direccion;
        destinatario.Referencia = dto.Referencia;

        await _destinatarioRepository.Actualizar(destinatario);
        await _destinatarioRepository.Guardar();

        return "Destinatario actualizado correctamente.";
    }

    public async Task<string> Desactivar(
        int id,
        int usuarioId,
        string rol,
        string? departamento,
        string? distrito)
    {
        var destinatario = await _destinatarioRepository.ObtenerPorId(id);

        if (destinatario == null)
        {
            return "Destinatario no encontrado.";
        }

        if (rol == "Admin")
        {
            if (destinatario.Departamento != departamento || destinatario.Distrito != distrito)
            {
                return "No puedes desactivar destinatarios fuera de tu zona.";
            }
        }

        if (rol == "Empresa")
        {
            if (destinatario.EmpresaCliente.UsuarioId != usuarioId)
            {
                return "No puedes desactivar destinatarios de otra empresa.";
            }
        }

        destinatario.Activo = false;

        await _destinatarioRepository.Actualizar(destinatario);
        await _destinatarioRepository.Guardar();

        return "Destinatario desactivado correctamente.";
    }

    private List<object> MapearDestinatarios(List<Destinatario> destinatarios)
    {
        return destinatarios.Select(d => new
        {
            d.Id,
            d.Nombre,
            d.Telefono,
            d.Departamento,
            d.Distrito,
            d.Direccion,
            d.Referencia,
            d.Activo,
            Empresa = d.EmpresaCliente.NombreEmpresa
        }).Cast<object>().ToList();
    }
}