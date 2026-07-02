using API_Envios.DTOs;
using API_Envios.Data;
using API_Envios.Models;
using API_Envios.Security;
using API_Envios.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Envios.Controllers;

[ApiController]
[Authorize(Roles = "AdministradorMaster,Administrador")]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public UsuariosController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UsuarioResponse>> GetAll()
    {
        var usuarios = _db.Usuarios.AsNoTracking().AsEnumerable();
        if (User.IsInRole(RolUsuario.Administrador.ToString()))
        {
            usuarios = usuarios.Where(CanManageUsuario);
        }

        return Ok(usuarios.Select(DataMapper.ToResponse));
    }

    [HttpGet("{id:int}")]
    public ActionResult<UsuarioResponse> GetById(int id)
    {
        var user = _db.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == id);
        if (user is not null && !CanManageUsuario(user))
        {
            return Forbid();
        }

        return user is null ? NotFound() : Ok(DataMapper.ToResponse(user));
    }

    [HttpPost]
    public ActionResult<UsuarioResponse> Create(UsuarioCreateRequest request)
    {
        if (_db.Usuarios.Any(u => u.Correo == request.Correo))
        {
            return BadRequest("Ya existe un usuario con ese correo.");
        }

        if (!CanCreateUsuario(request))
        {
            return Forbid();
        }

        var validation = ValidateRoleLinks(request.Rol, request.EmpresaId, request.PilotoId);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var user = new Usuario
        {
            Nombre = request.Nombre,
            Correo = request.Correo,
            Password = request.Password,
            Rol = request.Rol,
            Departamento = request.Departamento,
            Distrito = request.Distrito,
            DireccionCercana = request.DireccionCercana,
            EmpresaId = request.EmpresaId,
            PilotoId = request.PilotoId
        };

        _db.Usuarios.Add(user);
        _db.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, DataMapper.ToResponse(user));
    }

    [HttpPut("{id:int}")]
    public ActionResult<UsuarioResponse> Update(int id, UsuarioUpdateRequest request)
    {
        var user = _db.Usuarios.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        if (!CanManageUsuario(user))
        {
            return Forbid();
        }

        if (_db.Usuarios.Any(u => u.Id != id && u.Correo == request.Correo))
        {
            return BadRequest("Ya existe otro usuario con ese correo.");
        }

        var validation = ValidateRoleLinks(user.Rol, request.EmpresaId, request.PilotoId);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        if (!CanUpdateUsuario(user.Rol, request))
        {
            return Forbid();
        }

        user.Nombre = request.Nombre;
        user.Correo = request.Correo;
        user.Activo = request.Activo;
        user.Departamento = request.Departamento;
        user.Distrito = request.Distrito;
        user.DireccionCercana = request.DireccionCercana;
        user.EmpresaId = request.EmpresaId;
        user.PilotoId = request.PilotoId;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.Password = request.Password;
        }

        _db.SaveChanges();
        return Ok(DataMapper.ToResponse(user));
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var user = _db.Usuarios.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        if (!CanManageUsuario(user))
        {
            return Forbid();
        }

        if (user.Rol is RolUsuario.EmpresaCliente or RolUsuario.Piloto)
        {
            user.Activo = false;
            _db.SaveChanges();
            return Ok("Usuario dado de baja.");
        }

        _db.Usuarios.Remove(user);
        _db.SaveChanges();
        return NoContent();
    }

    private string? ValidateRoleLinks(RolUsuario rol, int? empresaId, int? pilotoId)
    {
        if (rol == RolUsuario.AdministradorMaster && (empresaId.HasValue || pilotoId.HasValue))
        {
            return "El administrador master no debe vincularse a empresa ni piloto.";
        }

        if (rol == RolUsuario.Administrador && (empresaId.HasValue || pilotoId.HasValue))
        {
            return "El administrador no debe vincularse a empresa ni piloto.";
        }

        if (rol == RolUsuario.EmpresaCliente && (!empresaId.HasValue || !_db.Empresas.Any(e => e.Id == empresaId && e.Activa)))
        {
            return "El usuario empresa debe vincularse a una empresa activa.";
        }

        if (rol == RolUsuario.Piloto && (!pilotoId.HasValue || !_db.Pilotos.Any(p => p.Id == pilotoId && p.Activo)))
        {
            return "El usuario piloto debe vincularse a un piloto activo.";
        }

        return null;
    }

    private bool CanCreateUsuario(UsuarioCreateRequest request)
    {
        if (AuthorizationRules.IsMaster(User))
        {
            return ValidateAdminLocation(request.Rol, request.Departamento, request.Distrito, request.DireccionCercana);
        }

        if (!AuthorizationRules.IsAdministrador(User) || request.Rol == RolUsuario.AdministradorMaster)
        {
            return false;
        }

        if (!ValidateAdminLocation(request.Rol, request.Departamento, request.Distrito, request.DireccionCercana))
        {
            return false;
        }

        return request.Rol switch
        {
            RolUsuario.Administrador => AuthorizationRules.IsAdminInScope(User, _db, request.Departamento ?? string.Empty, request.Distrito ?? string.Empty),
            RolUsuario.EmpresaCliente => request.EmpresaId.HasValue && CanManageEmpresa(request.EmpresaId.Value),
            RolUsuario.Piloto => request.PilotoId.HasValue && CanManagePiloto(request.PilotoId.Value),
            _ => false
        };
    }

    private bool CanManageUsuario(Usuario usuario)
    {
        if (AuthorizationRules.IsMaster(User))
        {
            return true;
        }

        if (!AuthorizationRules.IsAdministrador(User) || usuario.Rol == RolUsuario.AdministradorMaster)
        {
            return false;
        }

        return usuario.Rol switch
        {
            RolUsuario.Administrador => AuthorizationRules.IsAdminInScope(User, _db, usuario.Departamento ?? string.Empty, usuario.Distrito ?? string.Empty),
            RolUsuario.EmpresaCliente => usuario.EmpresaId.HasValue && CanManageEmpresa(usuario.EmpresaId.Value),
            RolUsuario.Piloto => usuario.PilotoId.HasValue && CanManagePiloto(usuario.PilotoId.Value),
            _ => false
        };
    }

    private bool CanUpdateUsuario(RolUsuario rol, UsuarioUpdateRequest request)
    {
        if (!ValidateAdminLocation(rol, request.Departamento, request.Distrito, request.DireccionCercana))
        {
            return false;
        }

        if (AuthorizationRules.IsMaster(User))
        {
            return true;
        }

        return rol switch
        {
            RolUsuario.Administrador => AuthorizationRules.IsAdminInScope(User, _db, request.Departamento ?? string.Empty, request.Distrito ?? string.Empty),
            RolUsuario.EmpresaCliente => request.EmpresaId.HasValue && CanManageEmpresa(request.EmpresaId.Value),
            RolUsuario.Piloto => request.PilotoId.HasValue && CanManagePiloto(request.PilotoId.Value),
            _ => false
        };
    }

    private bool CanManageEmpresa(int empresaId)
    {
        var empresa = _db.Empresas.FirstOrDefault(e => e.Id == empresaId);
        return empresa is not null && AuthorizationRules.CanManageEmpresa(User, _db, empresa);
    }

    private bool CanManagePiloto(int pilotoId)
    {
        var piloto = _db.Pilotos.FirstOrDefault(p => p.Id == pilotoId);
        return piloto is not null && AuthorizationRules.CanManagePiloto(User, _db, piloto);
    }

    private static bool ValidateAdminLocation(RolUsuario rol, string? departamento, string? distrito, string? direccionCercana)
    {
        if (rol == RolUsuario.AdministradorMaster)
        {
            return true;
        }

        if (rol != RolUsuario.Administrador)
        {
            return true;
        }

        return !string.IsNullOrWhiteSpace(departamento) &&
            !string.IsNullOrWhiteSpace(distrito) &&
            !string.IsNullOrWhiteSpace(direccionCercana);
    }
}
