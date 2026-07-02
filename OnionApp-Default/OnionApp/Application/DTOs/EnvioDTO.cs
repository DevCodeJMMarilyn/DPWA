namespace Application.DTOs;

public class EnvioDTO
{
    public int EmpresaClienteId { get; set; }

    public int DestinatarioId { get; set; }

    public string Producto { get; set; } = string.Empty;

    public string DescripcionProducto { get; set; } = string.Empty;
}