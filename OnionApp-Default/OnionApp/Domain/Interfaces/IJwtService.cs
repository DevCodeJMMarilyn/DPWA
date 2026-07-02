using Domain.Entities;

namespace Domain.Interfaces;

public interface IJwtService
{
    string GenerarToken(Usuario usuario);
}