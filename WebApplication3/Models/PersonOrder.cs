using WebApplication3.Models;

public class PersonOrder
{
    public int PersonOrderID { get; set; }
    public int TableOrderID { get; set; }
    public string PersonName { get; set; }
    public decimal TotalPrice { get; set; }
    public int PaymentID { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
}