using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appweb2.Models
{
    public class Compra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        [Required]
        public int UsuarioID { get; set; }

        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; } = null!;

        [Required]
        public int VideoJuegoID { get; set; }
        [ForeignKey("VideoJuegoID")]
        public VideoJuego VideoJuego { get; set; } = null!;

        public ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
    }
}
