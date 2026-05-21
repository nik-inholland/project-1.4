namespace WebApplication3.Models
{
    public class PersonOrderViewModel
    {
        public int PersonOrderID { get; set; }
        public string PersonName { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderStatus { get; set; }
    }
}
