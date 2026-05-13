
namespace WebApplication3.Models
{
    public class OrderItem : IOrderItem
    {
        public int Item_id { get; set; }
        public string Dish_name { get; set; }
        public string Details { get; set; }
        public decimal Price { get; set; }
        public decimal VAT { get; set; }
        public int Stock { get; set; }
        public string Type { get; set; }

        public OrderItem(int intem_id, string dish_name, string details, decimal price, decimal vat, int stock, string type)
        {
            Item_id = intem_id;
            Dish_name = dish_name;
            Details = details;
            Price = price;
            VAT = vat;
            Stock = stock;
            Type = type;
        }
    }
}
