namespace API_Envios.Models;

public class Destinatario
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Distrito { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}
