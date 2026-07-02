using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/envios")]
[Authorize]
public class EnviosController : ControllerBase
{
    private readonly EnvioService _envioService;

    public EnviosController(EnvioService envioService)
    {
        _envioService = envioService;
    }

    [HttpGet]
    [Authorize(Roles = "Master,Admin,Empresa,Piloto")]
    public async Task<IActionResult> ObtenerTodos()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _envioService.ObtenerTodosPorRol(usuarioId, rol, departamento, distrito));
    }

    [HttpPost]
    [Authorize(Roles = "Master,Admin,Empresa")]
    public async Task<IActionResult> Crear(EnvioDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _envioService.Crear(dto, usuarioId, rol, departamento, distrito));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Master,Admin")]
    public async Task<IActionResult> Editar(int id, EnvioDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _envioService.Editar(id, dto, usuarioId, rol, departamento, distrito));
    }

    [HttpPost("asignar-piloto")]
    [Authorize(Roles = "Master,Admin")]
    public async Task<IActionResult> AsignarPiloto(AsignarPilotoDTO dto)
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _envioService.AsignarPiloto(dto, rol, departamento, distrito));
    }

    [HttpPut("cambiar-estado")]
    [Authorize(Roles = "Master,Admin,Piloto")]
    public async Task<IActionResult> CambiarEstado(CambiarEstadoDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _envioService.CambiarEstado(dto, usuarioId, rol, departamento, distrito));
    }

    [HttpPost("reportar-entrega")]
    [Authorize(Roles = "Master,Piloto")]
    public async Task<IActionResult> ReportarEntrega(EntregaDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;

        return Ok(await _envioService.ReportarEntrega(dto, usuarioId, rol));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Master,Admin")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _envioService.Desactivar(id, rol, departamento, distrito));
    }
}