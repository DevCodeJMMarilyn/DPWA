namespace Domain.Entities;

public class EmpresaCliente
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public string NombreEmpresa { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string Departamento { get; set; } = string.Empty;

    public string Distrito { get; set; } = string.Empty;

    public bool Activa { get; set; } = true;

    public ICollection<Destinatario> Destinatarios { get; set; } = new List<Destinatario>();

    public ICollection<Envio> Envios { get; set; } = new List<Envio>();
}