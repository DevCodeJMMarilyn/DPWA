namespace API_Envios.DTOs;

public record PilotoRequest(
    string Nombre,
    string Documento,
    string Telefono,
    string Departamento,
    string Distrito);

public record PilotoResponse(
    int Id,
    string Nombre,
    string Documento,
    string Telefono,
    string Departamento,
    string Distrito,
    bool Activo);
