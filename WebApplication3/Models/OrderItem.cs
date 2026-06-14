using System;

namespace WebApplication3.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }

        public int MenuItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal PricePerItem { get; set; }
        public int Quantity { get; set; }
        public string? Comments { get; set; }

        public int? VatCategory { get; set; }
        public int? Category { get; set; }
        public int ItemStatus { get; set; } = 0;

        public DateTime? PlacedAt { get; set; }
        public int? PersonOrderID { get; set; }

        public OrderItem() { }

        public OrderItem(int menuItemID, string itemName, decimal pricePerItem, int quantity,
                        string? comments = null, int? vatCategory = null, int? category = null)
        {
            MenuItemID = menuItemID;
            ItemName = itemName;
            PricePerItem = pricePerItem;
            Quantity = quantity;
            Comments = comments;
            VatCategory = vatCategory;
            Category = category;
            PlacedAt = DateTime.Now;
        }
    }
}