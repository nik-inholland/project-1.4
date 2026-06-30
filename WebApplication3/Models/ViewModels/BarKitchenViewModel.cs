using WebApplication3.Models;

namespace WebApplication3.Models.ViewModels
{
    
    public class BarKitchenViewModel
    {
        public int TableOrderID { get; set; }
        public int TableNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public TimeSpan WaitingTime => DateTime.Now - CreatedAt;
        public int WaitingMinutes => (int)WaitingTime.TotalMinutes;

        public List<BarKitchenCourseGroup> Courses { get; set; } = new List<BarKitchenCourseGroup>();

        public List<BarKitchenItemViewModel> AllItems =>
            Courses.SelectMany(c => c.Items).ToList();

        public string StatusCssClass
        {
            get
            {
                switch (OrderStatus)
                {
                    case OrderStatus.Ordered: return "ordered";
                    case OrderStatus.BeingPrepared: return "preparing";
                    case OrderStatus.ReadyToBeServed: return "ready";
                    case OrderStatus.Served: return "served";
                    case OrderStatus.Cancelled: return "cancelled";
                    default: return "ordered";
                }
            }
        }
    }

    public class BarKitchenCourseGroup
    {
        public int CourseBucket { get; set; }     
        public string CourseName { get; set; } = "";
        public List<BarKitchenItemViewModel> Items { get; set; } = new List<BarKitchenItemViewModel>();
    }

    public class BarKitchenItemViewModel
    {
        public int OrderItemID { get; set; }
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public string? Comments { get; set; }
        public OrderStatus ItemStatus { get; set; }
        public int CourseType { get; set; }
    }
}
