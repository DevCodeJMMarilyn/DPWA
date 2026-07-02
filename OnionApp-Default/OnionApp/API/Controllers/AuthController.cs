using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var resultado = await _authService.Login(dto);

        if (resultado == null)
        {
            return Unauthorized("Credenciales incorrectas o usuario inactivo.");
        }

        return Ok(resultado);
    }
}