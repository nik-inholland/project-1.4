namespace WebApplication3.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int MenuItemID { get; set; }

        // Navigation property
        public MenuItem? MenuItem { get; set; }

        public int Quantity { get; set; } = 1;
        public string? Comment { get; set; }

        // Keep your existing fields if they are used elsewhere
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public bool VatCategory { get; set; }
        public int CourseType { get; set; }

        // Fixed TotalPrice
        public decimal TotalPrice => (MenuItem?.Price ?? Price) * Quantity;

        public OrderItem() { }

        public OrderItem(
            int menuItemID,
            string description,
            decimal price,           // Changed to decimal
            bool vatCategory,
            int courseType,
            int quantity)
        {
            MenuItemID = menuItemID;
            Description = description;
            Price = price;
            VatCategory = vatCategory;
            CourseType = courseType;
            Quantity = quantity;
        }
    }
}