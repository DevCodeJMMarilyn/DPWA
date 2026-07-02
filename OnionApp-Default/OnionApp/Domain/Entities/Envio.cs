using Domain.Enums;

namespace Domain.Entities;

public class Envio
{
    public int Id { get; set; }

    public int EmpresaClienteId { get; set; }

    public EmpresaCliente EmpresaCliente { get; set; } = null!;

    public int DestinatarioId { get; set; }

    public Destinatario Destinatario { get; set; } = null!;

    public int? PilotoId { get; set; }

    public Piloto? Piloto { get; set; }

    public string Producto { get; set; } = string.Empty;

    public string DescripcionProducto { get; set; } = string.Empty;

    public EstadoEnvio Estado { get; set; } = EstadoEnvio.Recolectado;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaAsignacion { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public bool Activo { get; set; } = true;

    public Entrega? Entrega { get; set; }
}