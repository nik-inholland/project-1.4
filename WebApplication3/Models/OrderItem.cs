using WebApplication3.Models;
namespace WebApplication3.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int MenuItemID { get; set; }
        public MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }
        public string? Comments { get; set; }
        public OrderStatus itemStatus { get; set; } = 0;
        public DateTime PlacedAt { get; set; }

        public OrderItem(MenuItem menuItem, int quantity, string? comments = null)
        {
            MenuItem = menuItem ?? throw new ArgumentNullException(nameof(menuItem));
            MenuItemID = menuItem.MenuItemID;
            Quantity = quantity;
            Comments = comments;
            itemStatus = 0;
            PlacedAt = DateTime.Now;
        }

        public OrderItem() { }
        public decimal LineTotal => Quantity * (MenuItem?.Price ?? 0);
    }
}