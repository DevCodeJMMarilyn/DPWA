namespace Domain.Entities;

public class Piloto
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public string Licencia { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Departamento { get; set; } = string.Empty;

    public string Distrito { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<Envio> EnviosAsignados { get; set; } = new List<Envio>();
}