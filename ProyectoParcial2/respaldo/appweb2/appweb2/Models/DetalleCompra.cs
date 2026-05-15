using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appweb2.Models
{
    [Table("detalle_compra")]
    public class DetalleCompra
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(200)]
        public string VideoJuegosId { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(typeof(decimal), "0", "9999999999")]
        public decimal total { get; set; }

        [Required]
        [StringLength(50)]
        public string estadoCompra { get; set; } = string.Empty;

        public DateTime fechaHoraTransaccion { get; set; }

        [Required]
        [StringLength(100)]
        public string codigoTransaccion { get; set; } = string.Empty;

        public int idCompra { get; set; }

        [ForeignKey("idCompra")]
        public Compra Compra { get; set; } = null!;
    }
}
