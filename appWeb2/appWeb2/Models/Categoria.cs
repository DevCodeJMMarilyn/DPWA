using System.ComponentModel.DataAnnotations;

namespace appWeb2.Models
{
    public class Categoria
    {
        [Key] 
        public int idcategoria { get; set; }
        public string categoria {  get; set; }
    }
}
