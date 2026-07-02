namespace API_Envios.Models;

public class Piloto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Distrito { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
