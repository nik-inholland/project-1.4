namespace WebApplication3.Models
{
    public class TakeOrderViewModel
    {
        public int TableID { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new();
        public List<CurrentOrderItem> CurrentOrder { get; set; } = new();

        public string FilterCardType { get; set; } = "All";
        public string FilterCategory { get; set; } = "All";
    }

    public class CurrentOrderItem
    {
        public int MenuItemID { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public string? Comment { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}