using System;

namespace WebApplication3.Models
{
    public class MenuItem
    {
        public int MenuItemID { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool VatCategory { get; set; }
        public int CourseType { get; set; }
        public int QuantityInStock { get; set; }
        public string CardType { get; set; } = "All";

        public MenuItem() { }

        public MenuItem(int menuItemID, string description, decimal price, bool vatCategory,
                       int courseType, int quantityInStock, string cardType)
        {
            MenuItemID = menuItemID;
            Description = description;
            Price = price;
            VatCategory = vatCategory;
            CourseType = courseType;
            QuantityInStock = quantityInStock;
            CardType = cardType;
        }

        public bool IsOutOfStock() => QuantityInStock <= 0;
        public bool IsAlmostOutOfStock() => QuantityInStock > 0 && QuantityInStock <= 10;
    }
}