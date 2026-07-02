using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Policy = "SoloMaster")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        return Ok(await _usuarioService.ObtenerTodos());
    }

    [HttpPost]
    public async Task<IActionResult> Crear(UsuarioDTO dto)
    {
        return Ok(await _usuarioService.Crear(dto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, UsuarioDTO dto)
    {
        return Ok(await _usuarioService.Editar(id, dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        return Ok(await _usuarioService.Desactivar(id));
    }
}