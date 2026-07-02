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
[Authorize(Roles = "AdministradorMaster,Administrador,EmpresaCliente")]
[Route("api/[controller]")]
public class DestinatariosController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public DestinatariosController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public ActionResult<IEnumerable<DestinatarioResponse>> GetAll()
    {
        var destinatarios = _db.Destinatarios.AsNoTracking().AsEnumerable();
        if (User.IsInRole(RolUsuario.EmpresaCliente.ToString()))
        {
            destinatarios = destinatarios.Where(d => d.EmpresaId == CurrentEmpresaId());
        }

        if (User.IsInRole(RolUsuario.Administrador.ToString()))
        {
            destinatarios = destinatarios.Where(d => AuthorizationRules.CanManageDestinatario(User, _db, d));
        }

        return Ok(destinatarios.Select(DataMapper.ToResponse));
    }

    [HttpGet("{id:int}")]
    public ActionResult<DestinatarioResponse> GetById(int id)
    {
        var destinatario = _db.Destinatarios.AsNoTracking().FirstOrDefault(d => d.Id == id);
        if (destinatario is null)
        {
            return NotFound();
        }

        if (User.IsInRole(RolUsuario.EmpresaCliente.ToString()) && destinatario.EmpresaId != CurrentEmpresaId())
        {
            return Forbid();
        }

        if (User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.CanManageDestinatario(User, _db, destinatario))
        {
            return Forbid();
        }

        return Ok(DataMapper.ToResponse(destinatario));
    }

    [HttpPost]
    public ActionResult<DestinatarioResponse> Create(DestinatarioRequest request)
    {
        var empresaId = ResolveEmpresaId(request.EmpresaId);
        if (empresaId is null)
        {
            return BadRequest("Debe indicar una empresa valida.");
        }

        if (User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.IsAdminInScope(User, _db, request.Departamento, request.Distrito))
        {
            return Forbid();
        }

        var destinatario = new Destinatario
        {
            EmpresaId = empresaId.Value,
            Nombre = request.Nombre,
            Telefono = request.Telefono,
            Departamento = request.Departamento,
            Distrito = request.Distrito,
            Direccion = request.Direccion
        };

        _db.Destinatarios.Add(destinatario);
        _db.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = destinatario.Id }, DataMapper.ToResponse(destinatario));
    }

    [HttpPut("{id:int}")]
    public ActionResult<DestinatarioResponse> Update(int id, DestinatarioRequest request)
    {
        var destinatario = _db.Destinatarios.FirstOrDefault(d => d.Id == id);
        if (destinatario is null)
        {
            return NotFound();
        }

        if (User.IsInRole(RolUsuario.EmpresaCliente.ToString()) && destinatario.EmpresaId != CurrentEmpresaId())
        {
            return Forbid();
        }

        if (User.IsInRole(RolUsuario.Administrador.ToString()) &&
            (!AuthorizationRules.CanManageDestinatario(User, _db, destinatario) ||
             !AuthorizationRules.IsAdminInScope(User, _db, request.Departamento, request.Distrito)))
        {
            return Forbid();
        }

        var empresaId = ResolveEmpresaId(request.EmpresaId);
        if (empresaId is null)
        {
            return BadRequest("Debe indicar una empresa valida.");
        }

        destinatario.EmpresaId = empresaId.Value;
        destinatario.Nombre = request.Nombre;
        destinatario.Telefono = request.Telefono;
        destinatario.Departamento = request.Departamento;
        destinatario.Distrito = request.Distrito;
        destinatario.Direccion = request.Direccion;

        _db.SaveChanges();
        return Ok(DataMapper.ToResponse(destinatario));
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var destinatario = _db.Destinatarios.FirstOrDefault(d => d.Id == id);
        if (destinatario is null)
        {
            return NotFound();
        }

        if (User.IsInRole(RolUsuario.EmpresaCliente.ToString()) && destinatario.EmpresaId != CurrentEmpresaId())
        {
            return Forbid();
        }

        if (User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.CanManageDestinatario(User, _db, destinatario))
        {
            return Forbid();
        }

        if (_db.Envios.Any(e => e.DestinatarioId == id))
        {
            return BadRequest("No se puede eliminar un destinatario con envios registrados.");
        }

        _db.Destinatarios.Remove(destinatario);
        _db.SaveChanges();
        return NoContent();
    }

    private int? ResolveEmpresaId(int? requestedEmpresaId)
    {
        var empresaId = User.IsInRole(RolUsuario.EmpresaCliente.ToString())
            ? CurrentEmpresaId()
            : requestedEmpresaId;

        return _db.Empresas.Any(e => e.Id == empresaId && e.Activa) ? empresaId : null;
    }

    private int? CurrentEmpresaId()
    {
        var value = User.FindFirst("empresaId")?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}
