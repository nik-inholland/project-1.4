using WebApplication3.Models;

namespace WebApplication3.Models.ViewModels
{
    public class OrderDetailViewModel
    {
        public int TableOrderID { get; set; }
        public int TableNumber { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal TotalPrice { get; set; }

        public List<OrderItemDetailViewModel> Items { get; set; } = new();
    }

    public class OrderItemDetailViewModel
    {
        public int OrderItemID { get; set; }

        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string? Comments { get; set; }
        public OrderStatus ItemStatus { get; set; }
    }
}