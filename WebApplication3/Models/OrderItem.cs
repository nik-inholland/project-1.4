namespace WebApplication3.Models
{
    public class OrderItem
    {
        public int MenuItemID { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public bool VatCategory { get; set; }

        public int CourseType { get; set; }

        public int Quantity { get; set; }

        public OrderItem()
        {
        }

        public OrderItem(
            int menuItemID,
            string description,
            double price,
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