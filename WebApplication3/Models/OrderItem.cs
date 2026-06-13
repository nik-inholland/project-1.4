using System.Data;

namespace WebApplication3.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }

        public string Name { get; set; }

        public string Comments { get; set; }

        public double Price { get; set; }

        public bool VatCategory { get; set; }

        public int Category { get; set; }

        public int Quantity { get; set; }

        public int MenuItemId { get; set; }
        
        public DateTime PlacedAt { get; set; }

        public int PersonOrderId { get; set; }

        public OrderItem()
        {
        }

        public OrderItem(int orderItemID, string name, string comment, double price, bool vatcategory, int category, int quantity, int menuitemid, DateTime placedat, int personalorderid)
        {
            OrderItemID = orderItemID;
            Name = name;
            Comments = comment;
            Price = price;
            VatCategory = vatcategory;
            Category = category;
            Quantity = quantity;
            MenuItemId = menuitemid;
            PlacedAt = placedat;
            PersonOrderId = personalorderid;
        }
    }
}