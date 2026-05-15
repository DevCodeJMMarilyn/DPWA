using System.Collections.Generic;

namespace appweb2.Models
{
    public class CatalogoVideoJuegosViewModel
    {
        public List<VideoJuego> Juegos { get; set; } = new();
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public string AccionActual { get; set; } = "Index";
        public int? CategoriaId { get; set; }
        public string Encabezado { get; set; } = "Videojuegos disponibles";
        public bool MostrarBanner { get; set; } = true;
    }
}
