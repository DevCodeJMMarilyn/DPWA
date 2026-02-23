using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appWeb2.Models
{
    public class VideoJuegos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string titulo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal precio { get; set; }

        [Required]
        [StringLength(100)]
        public string categoria { get; set; }

        [Required]
        [StringLength(500)]
        public string descripcion { get; set; }

        public ICollection<Compra> Compras { get; set; }

    }
}
