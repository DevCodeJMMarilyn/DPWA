using Application.DTOs;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUsuarioRepository usuarioRepository, IJwtService jwtService)
    {
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
    }

    public async Task<object?> Login(LoginDTO dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorEmail(dto.Email);

        if (usuario == null || !usuario.Activo)
        {
            return null;
        }

        var passwordCorrecta = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);

        if (!passwordCorrecta)
        {
            return null;
        }

        var token = _jwtService.GenerarToken(usuario);

        return new
        {
            mensaje = "Login correcto.",
            token,
            usuario = new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                Rol = usuario.Rol.ToString()
            }
        };
    }
}