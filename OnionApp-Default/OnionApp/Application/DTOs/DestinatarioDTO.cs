namespace Application.DTOs;

public class DestinatarioDTO
{
    public int EmpresaClienteId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Departamento { get; set; } = string.Empty;

    public string Distrito { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string Referencia { get; set; } = string.Empty;
}