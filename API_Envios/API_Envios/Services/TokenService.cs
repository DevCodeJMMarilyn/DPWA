using System.Security.Claims;
using API_Envios.Data;
using API_Envios.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API_Envios.Services;

public class TokenService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<string, int> _tokens = [];

    public TokenService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var correo = request.Correo.Trim().ToLower();
        var user = db.Usuarios.AsNoTracking().FirstOrDefault(u =>
            u.Activo &&
            u.Correo.ToLower() == correo &&
            u.Password == request.Password);

        if (user is null)
        {
            return null;
        }

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", string.Empty)
            .Replace("/", string.Empty)
            .Replace("=", string.Empty);

        _tokens[token] = user.Id;

        return new LoginResponse(token, user.Id, user.Nombre, user.Rol, user.EmpresaId, user.PilotoId);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (!_tokens.TryGetValue(token, out var userId))
        {
            return null;
        }

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = db.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == userId && u.Activo);
        if (user is null)
        {
            return null;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Nombre),
            new(ClaimTypes.Email, user.Correo),
            new(ClaimTypes.Role, user.Rol.ToString())
        };

        if (user.EmpresaId.HasValue)
        {
            claims.Add(new("empresaId", user.EmpresaId.Value.ToString()));
        }

        if (user.PilotoId.HasValue)
        {
            claims.Add(new("pilotoId", user.PilotoId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, "FlatBearer");
        return new ClaimsPrincipal(identity);
    }
}
