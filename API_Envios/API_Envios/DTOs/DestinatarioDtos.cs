namespace API_Envios.DTOs;

public record DestinatarioRequest(
    int? EmpresaId,
    string Nombre,
    string Telefono,
    string Departamento,
    string Distrito,
    string Direccion);

public record DestinatarioResponse(
    int Id,
    int EmpresaId,
    string Nombre,
    string Telefono,
    string Departamento,
    string Distrito,
    string Direccion);
