namespace WebApplication3.Models.ViewModels
{
    public class OrderListViewModel
    {
        public int TableOrderID { get; set; }
        public int TableNumber { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PersonCount { get; set; }
        public bool IsClosed { get; set; }
    }
}
