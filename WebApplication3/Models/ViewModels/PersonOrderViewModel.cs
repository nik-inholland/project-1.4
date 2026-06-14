namespace WebApplication3.Models.ViewModels
{
    public class PersonOrderViewModel
    {
        public int PersonOrderID { get; set; }
        public string PersonName { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();
    }
}
