using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/empresas")]
[Authorize(Policy = "MasterOAdmin")]
public class EmpresasController : ControllerBase
{
    private readonly EmpresaService _empresaService;

    public EmpresasController(EmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _empresaService.ObtenerTodasPorRol(rol, departamento, distrito));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(EmpresaDTO dto)
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        if (rol == "Admin")
        {
            if (dto.Departamento != departamento || dto.Distrito != distrito)
            {
                return Forbid("No puedes crear empresas fuera de tu zona.");
            }
        }

        return Ok(await _empresaService.Crear(dto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, EmpresaDTO dto)
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _empresaService.Editar(id, dto, rol, departamento, distrito));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DarDeBaja(int id)
    {
        var rol = User.FindFirst(ClaimTypes.Role)!.Value;
        var departamento = User.FindFirst("Departamento")?.Value;
        var distrito = User.FindFirst("Distrito")?.Value;

        return Ok(await _empresaService.DarDeBaja(id, rol, departamento, distrito));
    }
}