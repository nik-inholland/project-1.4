namespace WebApplication3.Models.ViewModels
{
    public class OrderItemViewModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int Category { get; set; }
        public decimal SubTotal => (decimal)(Price * Quantity);
    }
}
