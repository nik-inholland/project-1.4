namespace WebApplication3.Models.ViewModels
{
    public class OrderDetailViewModel
    {
        public int TableOrderID { get; set; }
        public int TableNumber { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public List<PersonOrderViewModel> PersonOrders { get; set; } = new();
    }
}
