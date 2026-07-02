using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class EmpresaService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public EmpresaService(IEmpresaRepository empresaRepository, IUsuarioRepository usuarioRepository)
    {
        _empresaRepository = empresaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<object>> ObtenerTodas()
    {
        var empresas = await _empresaRepository.ObtenerTodas();

        return MapearEmpresas(empresas);
    }

    public async Task<List<object>> ObtenerTodasPorRol(
        string rol,
        string? departamento,
        string? distrito)
    {
        var empresas = await _empresaRepository.ObtenerTodas();

        if (rol == "Admin")
        {
            empresas = empresas
                .Where(e =>
                    e.Departamento == departamento &&
                    e.Distrito == distrito)
                .ToList();
        }

        return MapearEmpresas(empresas);
    }

    public async Task<string> Crear(EmpresaDTO dto)
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
            Rol = RolUsuario.Empresa,
            Departamento = dto.Departamento,
            Distrito = dto.Distrito,
            DireccionCercana = dto.Direccion,
            Activo = true
        };

        var empresa = new EmpresaCliente
        {
            Usuario = usuario,
            NombreEmpresa = dto.NombreEmpresa,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion,
            Departamento = dto.Departamento,
            Distrito = dto.Distrito,
            Activa = true
        };

        await _empresaRepository.Crear(empresa);
        await _empresaRepository.Guardar();

        return "Empresa creada correctamente.";
    }

    public async Task<string> Editar(
        int id,
        EmpresaDTO dto,
        string rol,
        string? departamento,
        string? distrito)
    {
        var empresa = await _empresaRepository.ObtenerPorId(id);

        if (empresa == null)
        {
            return "Empresa no encontrada.";
        }

        if (rol == "Admin")
        {
            if (empresa.Departamento != departamento || empresa.Distrito != distrito)
            {
                return "No puedes editar empresas fuera de tu zona.";
            }

            if (dto.Departamento != departamento || dto.Distrito != distrito)
            {
                return "No puedes mover la empresa fuera de tu zona.";
            }
        }

        empresa.NombreEmpresa = dto.NombreEmpresa;
        empresa.Telefono = dto.Telefono;
        empresa.Direccion = dto.Direccion;
        empresa.Departamento = dto.Departamento;
        empresa.Distrito = dto.Distrito;

        empresa.Usuario.Nombre = dto.NombreUsuario;
        empresa.Usuario.Email = dto.Email;
        empresa.Usuario.Departamento = dto.Departamento;
        empresa.Usuario.Distrito = dto.Distrito;
        empresa.Usuario.DireccionCercana = dto.Direccion;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            empresa.Usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _empresaRepository.Actualizar(empresa);
        await _empresaRepository.Guardar();

        return "Empresa actualizada correctamente.";
    }

    public async Task<string> DarDeBaja(
        int id,
        string rol,
        string? departamento,
        string? distrito)
    {
        var empresa = await _empresaRepository.ObtenerPorId(id);

        if (empresa == null)
        {
            return "Empresa no encontrada.";
        }

        if (rol == "Admin")
        {
            if (empresa.Departamento != departamento || empresa.Distrito != distrito)
            {
                return "No puedes dar de baja empresas fuera de tu zona.";
            }
        }

        empresa.Activa = false;
        empresa.Usuario.Activo = false;

        await _empresaRepository.Actualizar(empresa);
        await _empresaRepository.Guardar();

        return "Empresa dada de baja correctamente.";
    }

    private List<object> MapearEmpresas(List<EmpresaCliente> empresas)
    {
        return empresas.Select(e => new
        {
            e.Id,
            e.NombreEmpresa,
            e.Telefono,
            e.Direccion,
            e.Departamento,
            e.Distrito,
            e.Activa,
            Usuario = new
            {
                e.Usuario.Id,
                e.Usuario.Nombre,
                e.Usuario.Email
            }
        }).Cast<object>().ToList();
    }
}