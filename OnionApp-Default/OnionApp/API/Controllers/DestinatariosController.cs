using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/destinatarios")]
[Authorize(Policy = "EmpresaOAdmin")]
public class DestinatariosController : ControllerBase
{
    private readonly DestinatarioService _destinatarioService;

    public DestinatariosController(DestinatarioService destinatarioService)
    {
        _destinatarioService = destinatarioService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _destinatarioService.ObtenerTodosPorRol(usuarioId, rol, departamento, distrito));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(DestinatarioDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _destinatarioService.Crear(dto, usuarioId, rol, departamento, distrito));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, DestinatarioDTO dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _destinatarioService.Editar(id, dto, usuarioId, rol, departamento, distrito));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _destinatarioService.Desactivar(id, usuarioId, rol, departamento, distrito));
    }
}