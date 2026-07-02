using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/pilotos")]
[Authorize(Policy = "MasterOAdmin")]
public class PilotosController : ControllerBase
{
    private readonly PilotoService _pilotoService;

    public PilotosController(PilotoService pilotoService)
    {
        _pilotoService = pilotoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _pilotoService.ObtenerTodosPorRol(rol, departamento, distrito));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(PilotoDTO dto)
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        if (rol == "Admin")
        {
            if (dto.Departamento != departamento || dto.Distrito != distrito)
            {
                return Forbid("No puedes crear pilotos fuera de tu zona.");
            }
        }

        return Ok(await _pilotoService.Crear(dto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, PilotoDTO dto)
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _pilotoService.Editar(id, dto, rol, departamento, distrito));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DarDeBaja(int id)
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _pilotoService.DarDeBaja(id, rol, departamento, distrito));
    }
}