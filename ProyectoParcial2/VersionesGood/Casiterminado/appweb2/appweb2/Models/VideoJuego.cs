using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appweb2.Models
{
    public class VideoJuego
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PrecioPromocion { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        public Categoria? Categoria { get; set; }


        [StringLength(500)]
        public string? Descripcion { get; set; }

        public string? Imagen { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public bool EnPromocion { get; set; } = false;

        [Required]
        [Range(0, 21)]
        public int EdadMinima { get; set; } = 0;

        //public ICollection<Compra> compras { get; set; } = new List<Compra>();
    }
}
