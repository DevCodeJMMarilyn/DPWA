using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appWeb2.Models
{
    [Table("VideoJuegos")]
    public class VideoJuegos
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200)]
        public string titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0, 9999.99, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        public decimal precio { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(100)]
        public string categoria { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? descripcion { get; set; }

        public string? imagen { get; set; }

        [Range(0, 99, ErrorMessage = "La edad mínima debe estar entre 0 y 99.")]
        public int edadMinima { get; set; } = 0;

        public bool enPromocion { get; set; } = false;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
