using System.ComponentModel.DataAnnotations;

namespace appweb2.Models
{
    public class GestionCategoriasViewModel
    {
        public List<Categoria> Categorias { get; set; } = new();

        [Required(ErrorMessage = "Ingresa un nombre para la categoria.")]
        [StringLength(100)]
        public string NuevaCategoriaNombre { get; set; } = string.Empty;
    }
}
