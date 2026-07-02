using API_Envios.Data;
using API_Envios.DTOs;
using API_Envios.Models;

namespace API_Envios.Services;

public static class DataMapper
{
    public static UsuarioResponse ToResponse(Usuario usuario) =>
        new(
            usuario.Id,
            usuario.Nombre,
            usuario.Correo,
            usuario.Rol,
            usuario.Activo,
            usuario.Departamento,
            usuario.Distrito,
            usuario.DireccionCercana,
            usuario.EmpresaId,
            usuario.PilotoId);

    public static EmpresaResponse ToResponse(Empresa empresa) =>
        new(empresa.Id, empresa.Nombre, empresa.Nit, empresa.Telefono, empresa.Correo, empresa.Departamento, empresa.Distrito, empresa.Direccion, empresa.Activa);

    public static PilotoResponse ToResponse(Piloto piloto) =>
        new(piloto.Id, piloto.Nombre, piloto.Documento, piloto.Telefono, piloto.Departamento, piloto.Distrito, piloto.Activo);

    public static DestinatarioResponse ToResponse(Destinatario destinatario) =>
        new(destinatario.Id, destinatario.EmpresaId, destinatario.Nombre, destinatario.Telefono, destinatario.Departamento, destinatario.Distrito, destinatario.Direccion);

    public static EnvioResponse ToResponse(Envio envio, ApplicationDbContext db)
    {
        var empresa = db.Empresas.FirstOrDefault(e => e.Id == envio.EmpresaId);
        var destinatario = db.Destinatarios.FirstOrDefault(d => d.Id == envio.DestinatarioId);
        var piloto = db.Pilotos.FirstOrDefault(p => p.Id == envio.PilotoId);

        return new EnvioResponse(
            envio.Id,
            envio.CodigoRastreo,
            envio.EmpresaId,
            empresa?.Nombre ?? "Empresa no encontrada",
            envio.DestinatarioId,
            destinatario?.Nombre ?? "Destinatario no encontrado",
            destinatario?.Direccion ?? string.Empty,
            envio.PilotoId,
            piloto?.Nombre,
            envio.DescripcionPedido,
            envio.PesoLibras,
            envio.Estado,
            envio.FechaCreacion,
            envio.FechaEntrega,
            envio.FirmaRecibido,
            envio.ImagenesEntrega,
            envio.Historial
                .OrderBy(h => h.Fecha)
                .Select(h => new HistorialEnvioResponse(h.Fecha, h.Estado, h.Comentario, h.UsuarioId, h.Usuario))
                .ToList());
    }
}
