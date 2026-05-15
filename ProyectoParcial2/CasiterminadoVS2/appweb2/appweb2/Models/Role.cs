using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appweb2.Models
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
