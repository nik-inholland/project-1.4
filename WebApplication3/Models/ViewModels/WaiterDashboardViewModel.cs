namespace WebApplication3.Models.ViewModels
{
    public class WaiterDashboardViewModel
    {
        public IEnumerable<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public string? ErrorMessage { get; set; }
    }
}