namespace appweb2.Models
{
    public class CarritoViewModel
    {
        public List<CarritoItemViewModel> Items { get; set; } = new();
        public string MetodoPago { get; set; } = "PayPal";

        public string PayPalOrderId { get; set; } = string.Empty;
        public string PayPalCaptureId { get; set; } = string.Empty;
        public string PayPalClientId { get; set; } = string.Empty;
        public string PayPalCurrency { get; set; } = "USD";
        public string PayPalAppName { get; set; } = "MishiGamesProject";

        public decimal Total => Items.Sum(x => x.Subtotal);
    }
}
