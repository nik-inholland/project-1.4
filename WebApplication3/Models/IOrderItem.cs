namespace WebApplication3.Models
{
    public interface IOrderItem
    {
        public int Item_id { get; set; }
        public string Dish_name { get; set; }
        public string Details { get; set; }
        public decimal Price { get; set; }
        public decimal VAT { get; set; }
        public int Stock { get; set; }
        public string Type { get; set; }
    }
}
