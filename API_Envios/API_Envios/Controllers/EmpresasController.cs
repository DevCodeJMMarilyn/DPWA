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
[Authorize]
[Route("api/[controller]")]
public class EmpresasController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public EmpresasController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public ActionResult<IEnumerable<EmpresaResponse>> GetAll()
    {
        var empresas = _db.Empresas.AsNoTracking().AsEnumerable();
        if (User.IsInRole(RolUsuario.Administrador.ToString()))
        {
            empresas = empresas.Where(e => AuthorizationRules.CanManageEmpresa(User, _db, e));
        }

        return Ok(empresas.Select(DataMapper.ToResponse));
    }

    [HttpGet("{id:int}")]
    public ActionResult<EmpresaResponse> GetById(int id)
    {
        if (User.IsInRole(RolUsuario.EmpresaCliente.ToString()) && CurrentEmpresaId() != id)
        {
            return Forbid();
        }

        var empresa = _db.Empresas.AsNoTracking().FirstOrDefault(e => e.Id == id);
        if (empresa is not null && User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.CanManageEmpresa(User, _db, empresa))
        {
            return Forbid();
        }

        return empresa is null ? NotFound() : Ok(DataMapper.ToResponse(empresa));
    }

    [HttpPost]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public ActionResult<EmpresaResponse> Create(EmpresaRequest request)
    {
        if (User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.IsAdminInScope(User, _db, request.Departamento, request.Distrito))
        {
            return Forbid();
        }

        var empresa = new Empresa
        {
            Nombre = request.Nombre,
            Nit = request.Nit,
            Telefono = request.Telefono,
            Correo = request.Correo,
            Departamento = request.Departamento,
            Distrito = request.Distrito,
            Direccion = request.Direccion
        };

        _db.Empresas.Add(empresa);
        _db.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, DataMapper.ToResponse(empresa));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public ActionResult<EmpresaResponse> Update(int id, EmpresaRequest request)
    {
        var empresa = _db.Empresas.FirstOrDefault(e => e.Id == id);
        if (empresa is null)
        {
            return NotFound();
        }

        if (!AuthorizationRules.CanManageEmpresa(User, _db, empresa) ||
            (User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.IsAdminInScope(User, _db, request.Departamento, request.Distrito)))
        {
            return Forbid();
        }

        empresa.Nombre = request.Nombre;
        empresa.Nit = request.Nit;
        empresa.Telefono = request.Telefono;
        empresa.Correo = request.Correo;
        empresa.Departamento = request.Departamento;
        empresa.Distrito = request.Distrito;
        empresa.Direccion = request.Direccion;

        _db.SaveChanges();
        return Ok(DataMapper.ToResponse(empresa));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public IActionResult DarDeBaja(int id)
    {
        var empresa = _db.Empresas.FirstOrDefault(e => e.Id == id);
        if (empresa is null)
        {
            return NotFound();
        }

        if (!AuthorizationRules.CanManageEmpresa(User, _db, empresa))
        {
            return Forbid();
        }

        empresa.Activa = false;
        _db.Usuarios.Where(u => u.EmpresaId == id).ToList().ForEach(u => u.Activo = false);
        _db.SaveChanges();
        return Ok("Empresa dada de baja sin eliminar historial.");
    }

    private int? CurrentEmpresaId()
    {
        var value = User.FindFirst("empresaId")?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}
