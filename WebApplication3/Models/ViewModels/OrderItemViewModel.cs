using WebApplication3.Models;

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