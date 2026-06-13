namespace WebApplication3.Models
{

    public class OrderTable
    {
        public int TableOrderID { get; set; }
        public decimal TotalPrice { get; set; }
        public int PaymentID { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<PersonOrder> PersonOrders { get; set; } = new();
    }
}