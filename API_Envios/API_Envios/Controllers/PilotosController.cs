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
public class PilotosController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PilotosController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public ActionResult<IEnumerable<PilotoResponse>> GetAll()
    {
        var pilotos = _db.Pilotos.AsNoTracking().AsEnumerable();
        if (User.IsInRole(RolUsuario.Administrador.ToString()))
        {
            pilotos = pilotos.Where(p => AuthorizationRules.CanManagePiloto(User, _db, p));
        }

        return Ok(pilotos.Select(DataMapper.ToResponse));
    }

    [HttpGet("{id:int}")]
    public ActionResult<PilotoResponse> GetById(int id)
    {
        if (User.IsInRole(RolUsuario.Piloto.ToString()) && CurrentPilotoId() != id)
        {
            return Forbid();
        }

        var piloto = _db.Pilotos.AsNoTracking().FirstOrDefault(p => p.Id == id);
        if (piloto is not null && User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.CanManagePiloto(User, _db, piloto))
        {
            return Forbid();
        }

        return piloto is null ? NotFound() : Ok(DataMapper.ToResponse(piloto));
    }

    [HttpPost]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public ActionResult<PilotoResponse> Create(PilotoRequest request)
    {
        if (User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.IsAdminInScope(User, _db, request.Departamento, request.Distrito))
        {
            return Forbid();
        }

        var piloto = new Piloto
        {
            Nombre = request.Nombre,
            Documento = request.Documento,
            Telefono = request.Telefono,
            Departamento = request.Departamento,
            Distrito = request.Distrito
        };

        _db.Pilotos.Add(piloto);
        _db.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = piloto.Id }, DataMapper.ToResponse(piloto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public ActionResult<PilotoResponse> Update(int id, PilotoRequest request)
    {
        var piloto = _db.Pilotos.FirstOrDefault(p => p.Id == id);
        if (piloto is null)
        {
            return NotFound();
        }

        if (!AuthorizationRules.CanManagePiloto(User, _db, piloto) ||
            (User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.IsAdminInScope(User, _db, request.Departamento, request.Distrito)))
        {
            return Forbid();
        }

        piloto.Nombre = request.Nombre;
        piloto.Documento = request.Documento;
        piloto.Telefono = request.Telefono;
        piloto.Departamento = request.Departamento;
        piloto.Distrito = request.Distrito;

        _db.SaveChanges();
        return Ok(DataMapper.ToResponse(piloto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public IActionResult DarDeBaja(int id)
    {
        var piloto = _db.Pilotos.FirstOrDefault(p => p.Id == id);
        if (piloto is null)
        {
            return NotFound();
        }

        if (!AuthorizationRules.CanManagePiloto(User, _db, piloto))
        {
            return Forbid();
        }

        piloto.Activo = false;
        _db.Usuarios.Where(u => u.PilotoId == id).ToList().ForEach(u => u.Activo = false);
        _db.SaveChanges();
        return Ok("Piloto dado de baja sin eliminar historial.");
    }

    private int? CurrentPilotoId()
    {
        var value = User.FindFirst("pilotoId")?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}
