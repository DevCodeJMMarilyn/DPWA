using System.Security.Claims;
using API_Envios.Data;
using API_Envios.Models;

namespace API_Envios.Security;

public static class AuthorizationRules
{
    public static int? CurrentUsuarioId(ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }

    public static Usuario? CurrentUsuario(ApplicationDbContext db, ClaimsPrincipal user)
    {
        var id = CurrentUsuarioId(user);
        return id.HasValue ? db.Usuarios.FirstOrDefault(u => u.Id == id.Value && u.Activo) : null;
    }

    public static bool IsMaster(ClaimsPrincipal user) =>
        user.IsInRole(RolUsuario.AdministradorMaster.ToString());

    public static bool IsAdministrador(ClaimsPrincipal user) =>
        user.IsInRole(RolUsuario.Administrador.ToString());

    public static bool IsAdminInScope(ClaimsPrincipal user, ApplicationDbContext db, string departamento, string distrito)
    {
        if (IsMaster(user))
        {
            return true;
        }

        if (!IsAdministrador(user))
        {
            return false;
        }

        var admin = CurrentUsuario(db, user);
        return Same(admin?.Departamento, departamento) && Same(admin?.Distrito, distrito);
    }

    public static bool CanManageEmpresa(ClaimsPrincipal user, ApplicationDbContext db, Empresa empresa) =>
        IsAdminInScope(user, db, empresa.Departamento, empresa.Distrito);

    public static bool CanManagePiloto(ClaimsPrincipal user, ApplicationDbContext db, Piloto piloto) =>
        IsAdminInScope(user, db, piloto.Departamento, piloto.Distrito);

    public static bool CanManageDestinatario(ClaimsPrincipal user, ApplicationDbContext db, Destinatario destinatario) =>
        IsAdminInScope(user, db, destinatario.Departamento, destinatario.Distrito);

    public static bool CanManageEnvio(ClaimsPrincipal user, ApplicationDbContext db, Envio envio)
    {
        if (IsMaster(user))
        {
            return true;
        }

        if (!IsAdministrador(user))
        {
            return false;
        }

        var destinatario = db.Destinatarios.FirstOrDefault(d => d.Id == envio.DestinatarioId);
        return destinatario is not null && CanManageDestinatario(user, db, destinatario);
    }

    private static bool Same(string? left, string? right) =>
        !string.IsNullOrWhiteSpace(left) &&
        !string.IsNullOrWhiteSpace(right) &&
        string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);
}
