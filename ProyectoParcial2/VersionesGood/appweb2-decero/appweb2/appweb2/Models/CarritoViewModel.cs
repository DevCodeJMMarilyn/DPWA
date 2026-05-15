using System.ComponentModel.DataAnnotations;

namespace appweb2.Models
{
    public class CarritoViewModel
    {
        public List<CarritoItemViewModel> Items { get; set; } = new();
        public string MetodoPago { get; set; } = "Povy";

        [Required(ErrorMessage = "Ingresa el numero de tarjeta.")]
        [StringLength(19, MinimumLength = 13, ErrorMessage = "La tarjeta debe tener entre 13 y 19 digitos.")]
        public string NumeroTarjeta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa el nombre del titular.")]
        [StringLength(100)]
        public string NombreTitular { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa el mes de expiracion.")]
        [RegularExpression("^(0[1-9]|1[0-2])$", ErrorMessage = "Usa un mes valido en formato MM.")]
        public string ExpMonth { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa el año de expiracion.")]
        [RegularExpression("^\\d{2}$", ErrorMessage = "Usa el año en formato YY.")]
        public string ExpYear { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa el CVV.")]
        [RegularExpression("^\\d{3,4}$", ErrorMessage = "El CVV debe tener 3 o 4 digitos.")]
        public string Cvv { get; set; } = string.Empty;

        public decimal Total => Items.Sum(x => x.Subtotal);
    }
}
