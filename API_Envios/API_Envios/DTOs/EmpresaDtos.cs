namespace API_Envios.DTOs;

public record EmpresaRequest(
    string Nombre,
    string Nit,
    string Telefono,
    string Correo,
    string Departamento,
    string Distrito,
    string Direccion);

public record EmpresaResponse(
    int Id,
    string Nombre,
    string Nit,
    string Telefono,
    string Correo,
    string Departamento,
    string Distrito,
    string Direccion,
    bool Activa);
