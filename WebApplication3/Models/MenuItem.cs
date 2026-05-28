namespace WebApplication3.Models
{
    public class MenuItem
    {
        public int MenuItemID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CardType { get; set; } = string.Empty; 
        public string Category { get; set; } = string.Empty;
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;

        public bool IsOutOfStock => Stock <= 0;
        public bool IsAlmostOutOfStock => Stock > 0 && Stock <= 10;
    }
}