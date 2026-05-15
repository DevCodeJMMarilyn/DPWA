using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appweb2.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<VideoJuego> VideoJuegos { get; set; } = new List<VideoJuego>();
    }
}
