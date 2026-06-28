namespace WebApplication3.Models
{ 
    public class OrderTable
    {
        public int TableOrderID { get; set; }
        public int TableNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public int PaymentID { get; set; }
        public OrderStatus orderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public bool IsClosed => ClosedAt != null;

        public decimal CalculateTotal() => OrderItems.Sum(i => i.LineTotal);

        public void Close()
        {
            ClosedAt = DateTime.Now;
            orderStatus = OrderStatus.Served;
        }
    }
}