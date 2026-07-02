using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiOne.Models
{
    // Representa un articulo musical digital de la tienda.
    public class MusicItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Artist { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? Genre { get; set; }

        public int ReleaseYear { get; set; }

        [Precision(10, 2)]
        public decimal Price { get; set; }

        public int Stock { get; set; }
    }
}
