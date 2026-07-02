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
public class ReportesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ReportesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("empresas")]
    public ActionResult<IEnumerable<ReporteEmpresaResponse>> GetReporteGeneral()
    {
        var empresas = _db.Empresas.AsNoTracking().AsEnumerable();
        if (User.IsInRole(RolUsuario.Administrador.ToString()))
        {
            empresas = empresas.Where(e => AuthorizationRules.CanManageEmpresa(User, _db, e));
        }

        return Ok(empresas.Select(BuildReporte));
    }

    [HttpGet("empresas/{empresaId:int}")]
    public ActionResult<ReporteEmpresaResponse> GetReportePorEmpresa(int empresaId)
    {
        var empresa = _db.Empresas.AsNoTracking().FirstOrDefault(e => e.Id == empresaId);
        if (empresa is not null && User.IsInRole(RolUsuario.Administrador.ToString()) && !AuthorizationRules.CanManageEmpresa(User, _db, empresa))
        {
            return Forbid();
        }

        return empresa is null ? NotFound() : Ok(BuildReporte(empresa));
    }

    private ReporteEmpresaResponse BuildReporte(Empresa empresa)
    {
        var envios = _db.Envios.AsNoTracking().Where(e => e.EmpresaId == empresa.Id).ToList();
        return new ReporteEmpresaResponse(
            empresa.Id,
            empresa.Nombre,
            envios.Count,
            envios.Count(e => e.Estado == EstadoEnvio.Recolectado),
            envios.Count(e => e.Estado == EstadoEnvio.EnBodega),
            envios.Count(e => e.Estado == EstadoEnvio.EnRuta),
            envios.Count(e => e.Estado == EstadoEnvio.Entregado),
            envios.Count(e => e.Estado == EstadoEnvio.Devolucion));
    }
}
