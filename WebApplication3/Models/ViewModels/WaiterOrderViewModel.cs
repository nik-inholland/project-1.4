using WebApplication3.Models;

namespace WebApplication3.Models.ViewModels
{
    public class WaiterOrderViewModel
    {
        public int TableNumber { get; set; }
        public OrderTable Order { get; set; } = new OrderTable();
        public decimal TotalPrice => Order?.OrderItems?.Sum(i => i.Quantity * i.MenuItem?.Price ?? 0) ?? 0;
    }
}