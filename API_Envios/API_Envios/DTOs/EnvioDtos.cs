using API_Envios.Models;

namespace API_Envios.DTOs;

public record EnvioCreateRequest(
    int? EmpresaId,
    int DestinatarioId,
    string DescripcionPedido,
    decimal PesoLibras);

public record EnvioUpdateRequest(
    int DestinatarioId,
    string DescripcionPedido,
    decimal PesoLibras);

public record AsignarPilotoRequest(int PilotoId);

public record ActualizarEstadoRequest(
    EstadoEnvio Estado,
    string Comentario,
    string? FirmaRecibido,
    List<string>? ImagenesEntrega);

public record HistorialEnvioResponse(
    DateTime Fecha,
    EstadoEnvio Estado,
    string Comentario,
    int? UsuarioId,
    string Usuario);

public record EnvioResponse(
    int Id,
    string CodigoRastreo,
    int EmpresaId,
    string Empresa,
    int DestinatarioId,
    string Destinatario,
    string DireccionEntrega,
    int? PilotoId,
    string? Piloto,
    string DescripcionPedido,
    decimal PesoLibras,
    EstadoEnvio Estado,
    DateTime FechaCreacion,
    DateTime? FechaEntrega,
    string? FirmaRecibido,
    List<string> ImagenesEntrega,
    List<HistorialEnvioResponse> Historial);

public record ReporteEmpresaResponse(
    int EmpresaId,
    string Empresa,
    int TotalEnvios,
    int Recolectados,
    int EnBodega,
    int EnRuta,
    int Entregados,
    int Devoluciones);
