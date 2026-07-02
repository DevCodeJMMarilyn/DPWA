using API_Envios.DTOs;
using API_Envios.Data;
using API_Envios.Models;
using API_Envios.Security;
using API_Envios.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_Envios.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EnviosController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public EnviosController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public ActionResult<IEnumerable<EnvioResponse>> GetAll()
    {
        var envios = FilterByRole(_db.Envios.Include(e => e.Historial)).ToList();
        return Ok(envios.Select(e => DataMapper.ToResponse(e, _db)));
    }

    [HttpGet("{id:int}")]
    public ActionResult<EnvioResponse> GetById(int id)
    {
        var envio = _db.Envios.Include(e => e.Historial).FirstOrDefault(e => e.Id == id);
        if (envio is null)
        {
            return NotFound();
        }

        if (!CanAccess(envio))
        {
            return Forbid();
        }

        return Ok(DataMapper.ToResponse(envio, _db));
    }

    [HttpGet("rastreo/{codigo}")]
    public ActionResult<EnvioResponse> GetByCodigo(string codigo)
    {
        var envio = _db.Envios.Include(e => e.Historial).FirstOrDefault(e => e.CodigoRastreo == codigo);
        if (envio is null)
        {
            return NotFound();
        }

        if (!CanAccess(envio))
        {
            return Forbid();
        }

        return Ok(DataMapper.ToResponse(envio, _db));
    }

    [HttpPost]
    [Authorize(Roles = "AdministradorMaster,Administrador,EmpresaCliente")]
    public ActionResult<EnvioResponse> Create(EnvioCreateRequest request)
    {
        var empresaId = User.IsInRole(RolUsuario.EmpresaCliente.ToString()) ? CurrentEmpresaId() : request.EmpresaId;
        if (!_db.Empresas.Any(e => e.Id == empresaId && e.Activa))
        {
            return BadRequest("Debe indicar una empresa activa.");
        }

        var destinatario = _db.Destinatarios.FirstOrDefault(d => d.Id == request.DestinatarioId && d.EmpresaId == empresaId);
        if (destinatario is null)
        {
            return BadRequest("El destinatario no existe o no pertenece a la empresa indicada.");
        }

        if (User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.CanManageDestinatario(User, _db, destinatario))
        {
            return Forbid();
        }

        var envio = new Envio
        {
            CodigoRastreo = $"ENV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            EmpresaId = empresaId!.Value,
            DestinatarioId = request.DestinatarioId,
            DescripcionPedido = request.DescripcionPedido,
            PesoLibras = request.PesoLibras,
            Estado = EstadoEnvio.Recolectado,
            Historial =
            [
                new()
                {
                    Estado = EstadoEnvio.Recolectado,
                    Comentario = "Envio creado y recolectado en origen.",
                    UsuarioId = CurrentUsuarioId(),
                    Usuario = User.Identity?.Name ?? "Sistema"
                }
            ]
        };

        _db.Envios.Add(envio);
        _db.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = envio.Id }, DataMapper.ToResponse(envio, _db));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public ActionResult<EnvioResponse> Update(int id, EnvioUpdateRequest request)
    {
        var envio = _db.Envios.Include(e => e.Historial).FirstOrDefault(e => e.Id == id);
        if (envio is null)
        {
            return NotFound();
        }

        if (!AuthorizationRules.CanManageEnvio(User, _db, envio))
        {
            return Forbid();
        }

        var destinatario = _db.Destinatarios.FirstOrDefault(d => d.Id == request.DestinatarioId && d.EmpresaId == envio.EmpresaId);
        if (destinatario is null)
        {
            return BadRequest("El destinatario no existe o no pertenece a la empresa del envio.");
        }

        if (!AuthorizationRules.CanManageDestinatario(User, _db, destinatario))
        {
            return Forbid();
        }

        envio.DestinatarioId = request.DestinatarioId;
        envio.DescripcionPedido = request.DescripcionPedido;
        envio.PesoLibras = request.PesoLibras;

        envio.Historial.Add(new HistorialEnvio
        {
            Estado = envio.Estado,
            Comentario = "Envio editado por administrador.",
            UsuarioId = CurrentUsuarioId(),
            Usuario = User.Identity?.Name ?? "Administrador"
        });

        _db.SaveChanges();
        return Ok(DataMapper.ToResponse(envio, _db));
    }

    [HttpPatch("{id:int}/asignar-piloto")]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public ActionResult<EnvioResponse> AsignarPiloto(int id, AsignarPilotoRequest request)
    {
        var envio = _db.Envios.Include(e => e.Historial).FirstOrDefault(e => e.Id == id);
        if (envio is null)
        {
            return NotFound();
        }

        if (!AuthorizationRules.CanManageEnvio(User, _db, envio))
        {
            return Forbid();
        }

        var piloto = _db.Pilotos.FirstOrDefault(p => p.Id == request.PilotoId && p.Activo);
        if (piloto is null)
        {
            return BadRequest("El piloto no existe o no esta activo.");
        }

        if (!AuthorizationRules.CanManagePiloto(User, _db, piloto))
        {
            return Forbid();
        }

        envio.PilotoId = piloto.Id;
        envio.Estado = EstadoEnvio.EnRuta;
        envio.Historial.Add(new HistorialEnvio
        {
            Estado = EstadoEnvio.EnRuta,
            Comentario = $"Asignado al piloto {piloto.Nombre}.",
            UsuarioId = CurrentUsuarioId(),
            Usuario = User.Identity?.Name ?? "Administrador"
        });

        _db.SaveChanges();
        return Ok(DataMapper.ToResponse(envio, _db));
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "AdministradorMaster,Administrador,Piloto")]
    public ActionResult<EnvioResponse> ActualizarEstado(int id, ActualizarEstadoRequest request)
    {
        var envio = _db.Envios.Include(e => e.Historial).FirstOrDefault(e => e.Id == id);
        if (envio is null)
        {
            return NotFound();
        }

        if (User.IsInRole(RolUsuario.Piloto.ToString()) && envio.PilotoId != CurrentPilotoId())
        {
            return Forbid();
        }

        if (User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.CanManageEnvio(User, _db, envio))
        {
            return Forbid();
        }

        if (request.Estado == EstadoEnvio.Entregado &&
            (string.IsNullOrWhiteSpace(request.FirmaRecibido) || request.ImagenesEntrega is null || request.ImagenesEntrega.Count == 0))
        {
            return BadRequest("Para marcar como entregado debe registrar firma e imagenes de la entrega.");
        }

        envio.Estado = request.Estado;
        envio.FirmaRecibido = request.FirmaRecibido ?? envio.FirmaRecibido;

        if (request.ImagenesEntrega is not null && request.ImagenesEntrega.Count > 0)
        {
            envio.ImagenesEntrega = request.ImagenesEntrega;
        }

        if (request.Estado == EstadoEnvio.Entregado)
        {
            envio.FechaEntrega = DateTime.UtcNow;
        }

        envio.Historial.Add(new HistorialEnvio
        {
            Estado = request.Estado,
            Comentario = request.Comentario,
            UsuarioId = CurrentUsuarioId(),
            Usuario = User.Identity?.Name ?? "Usuario"
        });

        _db.SaveChanges();
        return Ok(DataMapper.ToResponse(envio, _db));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "AdministradorMaster,Administrador")]
    public IActionResult Delete(int id)
    {
        var envio = _db.Envios.Include(e => e.Historial).FirstOrDefault(e => e.Id == id);
        if (envio is null)
        {
            return NotFound();
        }

        if (!AuthorizationRules.CanManageEnvio(User, _db, envio))
        {
            return Forbid();
        }

        _db.Envios.Remove(envio);
        _db.SaveChanges();
        return NoContent();
    }

    private IEnumerable<Envio> FilterByRole(IEnumerable<Envio> envios)
    {
        if (User.IsInRole(RolUsuario.EmpresaCliente.ToString()))
        {
            return envios.Where(e => e.EmpresaId == CurrentEmpresaId());
        }

        if (User.IsInRole(RolUsuario.Piloto.ToString()))
        {
            return envios.Where(e => e.PilotoId == CurrentPilotoId());
        }

        if (User.IsInRole(RolUsuario.Administrador.ToString()))
        {
            return envios.Where(e => AuthorizationRules.CanManageEnvio(User, _db, e));
        }

        return envios;
    }

    private bool CanAccess(Envio envio)
    {
        if (User.IsInRole(RolUsuario.EmpresaCliente.ToString()))
        {
            return envio.EmpresaId == CurrentEmpresaId();
        }

        if (User.IsInRole(RolUsuario.Piloto.ToString()))
        {
            return envio.PilotoId == CurrentPilotoId();
        }

        if (User.IsInRole(RolUsuario.Administrador.ToString()))
        {
            return AuthorizationRules.CanManageEnvio(User, _db, envio);
        }

        return true;
    }

    private int? CurrentEmpresaId()
    {
        var value = User.FindFirst("empresaId")?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }

    private int? CurrentPilotoId()
    {
        var value = User.FindFirst("pilotoId")?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }

    private int? CurrentUsuarioId()
    {
        return AuthorizationRules.CurrentUsuarioId(User);
    }
}
