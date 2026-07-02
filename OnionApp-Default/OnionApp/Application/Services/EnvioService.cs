using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class EnvioService
{
    private readonly IEnvioRepository _envioRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IDestinatarioRepository _destinatarioRepository;
    private readonly IPilotoRepository _pilotoRepository;

    public EnvioService(
        IEnvioRepository envioRepository,
        IEmpresaRepository empresaRepository,
        IDestinatarioRepository destinatarioRepository,
        IPilotoRepository pilotoRepository)
    {
        _envioRepository = envioRepository;
        _empresaRepository = empresaRepository;
        _destinatarioRepository = destinatarioRepository;
        _pilotoRepository = pilotoRepository;
    }

    public async Task<List<object>> ObtenerTodos()
    {
        var envios = await _envioRepository.ObtenerTodos();

        return MapearEnvios(envios);
    }

    public async Task<List<object>> ObtenerTodosPorRol(
        int usuarioId,
        string rol,
        string? departamento,
        string? distrito)
    {
        var envios = await _envioRepository.ObtenerTodos();

        if (rol == "Admin")
        {
            envios = envios
                .Where(e =>
                    e.Destinatario.Departamento == departamento &&
                    e.Destinatario.Distrito == distrito)
                .ToList();
        }

        if (rol == "Piloto")
        {
            envios = envios
                .Where(e =>
                    e.Piloto != null &&
                    e.Piloto.UsuarioId == usuarioId)
                .ToList();
        }

        if (rol == "Empresa")
        {
            envios = envios
                .Where(e =>
                    e.EmpresaCliente.UsuarioId == usuarioId)
                .ToList();
        }

        return MapearEnvios(envios);
    }

    public async Task<string> Crear(
        EnvioDTO dto,
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

        var destinatario = await _destinatarioRepository.ObtenerPorId(dto.DestinatarioId);

        if (destinatario == null || !destinatario.Activo)
        {
            return "Destinatario no encontrado o inactivo.";
        }

        if (destinatario.EmpresaClienteId != dto.EmpresaClienteId)
        {
            return "El destinatario no pertenece a esta empresa.";
        }

        if (rol == "Admin")
        {
            if (destinatario.Departamento != departamento || destinatario.Distrito != distrito)
            {
                return "No puedes crear envíos fuera de tu zona.";
            }
        }

        if (rol == "Empresa")
        {
            if (empresa.UsuarioId != usuarioId)
            {
                return "No puedes crear envíos para otra empresa.";
            }
        }

        var envio = new Envio
        {
            EmpresaClienteId = dto.EmpresaClienteId,
            DestinatarioId = dto.DestinatarioId,
            Producto = dto.Producto,
            DescripcionProducto = dto.DescripcionProducto,
            Estado = EstadoEnvio.Recolectado,
            FechaCreacion = DateTime.Now,
            Activo = true
        };

        await _envioRepository.Crear(envio);
        await _envioRepository.Guardar();

        return "Envío creado correctamente en estado Recolectado.";
    }

    public async Task<string> Editar(
        int id,
        EnvioDTO dto,
        int usuarioId,
        string rol,
        string? departamento,
        string? distrito)
    {
        var envio = await _envioRepository.ObtenerPorId(id);

        if (envio == null)
        {
            return "Envío no encontrado.";
        }

        if (rol == "Admin")
        {
            if (envio.Destinatario.Departamento != departamento || envio.Destinatario.Distrito != distrito)
            {
                return "No puedes editar envíos fuera de tu zona.";
            }
        }

        var empresa = await _empresaRepository.ObtenerPorId(dto.EmpresaClienteId);

        if (empresa == null || !empresa.Activa)
        {
            return "Empresa no encontrada o inactiva.";
        }

        var destinatario = await _destinatarioRepository.ObtenerPorId(dto.DestinatarioId);

        if (destinatario == null || !destinatario.Activo)
        {
            return "Destinatario no encontrado o inactivo.";
        }

        if (destinatario.EmpresaClienteId != dto.EmpresaClienteId)
        {
            return "El destinatario no pertenece a esta empresa.";
        }

        if (rol == "Admin")
        {
            if (destinatario.Departamento != departamento || destinatario.Distrito != distrito)
            {
                return "No puedes mover el envío a otra zona.";
            }
        }

        envio.EmpresaClienteId = dto.EmpresaClienteId;
        envio.DestinatarioId = dto.DestinatarioId;
        envio.Producto = dto.Producto;
        envio.DescripcionProducto = dto.DescripcionProducto;

        await _envioRepository.Actualizar(envio);
        await _envioRepository.Guardar();

        return "Envío actualizado correctamente.";
    }

    public async Task<string> AsignarPiloto(
        AsignarPilotoDTO dto,
        string rol,
        string? departamento,
        string? distrito)
    {
        var envio = await _envioRepository.ObtenerPorId(dto.EnvioId);

        if (envio == null || !envio.Activo)
        {
            return "Envío no encontrado o inactivo.";
        }

        var piloto = await _pilotoRepository.ObtenerPorId(dto.PilotoId);

        if (piloto == null || !piloto.Activo)
        {
            return "Piloto no encontrado o inactivo.";
        }

        if (rol == "Admin")
        {
            if (envio.Destinatario.Departamento != departamento || envio.Destinatario.Distrito != distrito)
            {
                return "No puedes asignar pilotos a envíos fuera de tu zona.";
            }

            if (piloto.Departamento != departamento || piloto.Distrito != distrito)
            {
                return "No puedes asignar pilotos fuera de tu zona.";
            }
        }

        envio.PilotoId = dto.PilotoId;
        envio.Estado = EstadoEnvio.EnRuta;
        envio.FechaAsignacion = DateTime.Now;

        await _envioRepository.Actualizar(envio);
        await _envioRepository.Guardar();

        return "Piloto asignado correctamente. Estado actualizado a En Ruta.";
    }

    public async Task<string> CambiarEstado(
        CambiarEstadoDTO dto,
        int usuarioId,
        string rol,
        string? departamento,
        string? distrito)
    {
        var envio = await _envioRepository.ObtenerPorId(dto.EnvioId);

        if (envio == null || !envio.Activo)
        {
            return "Envío no encontrado o inactivo.";
        }

        if (rol == "Admin")
        {
            if (envio.Destinatario.Departamento != departamento || envio.Destinatario.Distrito != distrito)
            {
                return "No puedes cambiar el estado de envíos fuera de tu zona.";
            }
        }

        if (rol == "Piloto")
        {
            if (envio.Piloto == null || envio.Piloto.UsuarioId != usuarioId)
            {
                return "No puedes cambiar el estado de un envío que no está asignado a ti.";
            }
        }

        envio.Estado = dto.Estado;

        if (dto.Estado == EstadoEnvio.Entregado)
        {
            envio.FechaEntrega = DateTime.Now;
        }

        await _envioRepository.Actualizar(envio);
        await _envioRepository.Guardar();

        return $"Estado actualizado correctamente a {dto.Estado}.";
    }

    public async Task<string> ReportarEntrega(EntregaDTO dto, int usuarioId, string rol)
    {
        var envio = await _envioRepository.ObtenerPorId(dto.EnvioId);

        if (envio == null || !envio.Activo)
        {
            return "Envío no encontrado o inactivo.";
        }

        if (envio.PilotoId == null || envio.Piloto == null)
        {
            return "Este envío no tiene piloto asignado.";
        }

        // Si es Piloto, solo puede reportar sus propios envíos
        if (rol == "Piloto")
        {
            if (envio.Piloto.UsuarioId != usuarioId)
            {
                return "No puedes reportar una entrega que no está asignada a ti.";
            }
        }

        // Si ya existe entrega, la actualizamos
        if (envio.Entrega != null)
        {
            envio.Entrega.FirmaCliente = dto.FirmaCliente;
            envio.Entrega.Imagen1 = dto.Imagen1;
            envio.Entrega.Imagen2 = dto.Imagen2;
            envio.Entrega.Observaciones = dto.Observaciones;
            envio.Entrega.FechaEntrega = DateTime.Now;

            envio.Estado = EstadoEnvio.Entregado;
            envio.FechaEntrega = DateTime.Now;

            await _envioRepository.Actualizar(envio);
            await _envioRepository.Guardar();

            return "Entrega actualizada correctamente.";
        }

        // Si no existe entrega, la creamos
        var entrega = new Entrega
        {
            EnvioId = dto.EnvioId,
            FirmaCliente = dto.FirmaCliente,
            Imagen1 = dto.Imagen1,
            Imagen2 = dto.Imagen2,
            Observaciones = dto.Observaciones,
            FechaEntrega = DateTime.Now
        };

        envio.Estado = EstadoEnvio.Entregado;
        envio.FechaEntrega = DateTime.Now;

        await _envioRepository.CrearEntrega(entrega);
        await _envioRepository.Actualizar(envio);
        await _envioRepository.Guardar();

        return "Entrega reportada correctamente con firma e imágenes.";
    }

    public async Task<string> Desactivar(
        int id,
        string rol,
        string? departamento,
        string? distrito)
    {
        var envio = await _envioRepository.ObtenerPorId(id);

        if (envio == null)
        {
            return "Envío no encontrado.";
        }

        if (rol == "Admin")
        {
            if (envio.Destinatario.Departamento != departamento || envio.Destinatario.Distrito != distrito)
            {
                return "No puedes desactivar envíos fuera de tu zona.";
            }
        }

        envio.Activo = false;

        await _envioRepository.Actualizar(envio);
        await _envioRepository.Guardar();

        return "Envío desactivado correctamente.";
    }

    private List<object> MapearEnvios(List<Envio> envios)
    {
        return envios.Select(e => new
        {
            e.Id,
            Empresa = e.EmpresaCliente.NombreEmpresa,
            Destinatario = e.Destinatario.Nombre,
            DepartamentoDestino = e.Destinatario.Departamento,
            DistritoDestino = e.Destinatario.Distrito,
            e.Producto,
            e.DescripcionProducto,
            Estado = e.Estado.ToString(),
            Piloto = e.Piloto != null ? e.Piloto.Usuario.Nombre : "Sin asignar",
            e.FechaCreacion,
            e.FechaAsignacion,
            e.FechaEntrega,
            e.Activo
        }).Cast<object>().ToList();
    }
}