namespace WebApplication3.Models
{
    public class OrderDetailsViewModel
    {
        public int TableOrderID { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderStatus { get; set; }
        public DateTime OrderDateTime { get; set; }

        public List<PersonOrderViewModel> Persons { get; set; }
    }
}
