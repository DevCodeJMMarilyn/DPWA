namespace API_Envios.Models;

public class Empresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Nit { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Distrito { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;
}
