namespace appweb2.Models
{
    public class CarritoItemViewModel
    {
        public int VideoJuegoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Imagen { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}
