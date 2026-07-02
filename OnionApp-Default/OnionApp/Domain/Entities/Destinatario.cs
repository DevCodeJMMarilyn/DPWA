namespace Domain.Entities;

public class Destinatario
{
    public int Id { get; set; }

    public int EmpresaClienteId { get; set; }

    public EmpresaCliente EmpresaCliente { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Departamento { get; set; } = string.Empty;

    public string Distrito { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string Referencia { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<Envio> Envios { get; set; } = new List<Envio>();
}