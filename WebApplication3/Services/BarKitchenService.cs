using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.repo.@interface;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services
{
    public class BarKitchenService : IBarKitchenService
    {
        private readonly IBarKitchenRepository _repo;

        public BarKitchenService(IBarKitchenRepository repo)
        {
            _repo = repo;
        }

        private static (int min, int max) Range(BarKitchenStation station)
        {
            switch (station)
            {
                case BarKitchenStation.Kitchen: return (1, 7);
                case BarKitchenStation.Bar: return (8, 13);
                default: return (1, 13);
            }
        }

        private const int BucketStarters = 1;
        private const int BucketMains = 2;
        private const int BucketDesserts = 3;
        private const int BucketDrinks = 0;

        private static readonly int[] StarterCourses = { 1, 4, 5 }; 
        private static readonly int[] MainCourses = { 2, 6 };    
        private static readonly int[] DessertCourses = { 3, 7 };   

        private static int BucketOf(int courseType)
        {
            if (StarterCourses.Contains(courseType)) return BucketStarters;
            if (MainCourses.Contains(courseType)) return BucketMains;
            if (DessertCourses.Contains(courseType)) return BucketDesserts;
            return BucketDrinks; 
        }

        private static string BucketName(int bucket)
        {
            switch (bucket)
            {
                case BucketStarters: return "Starters";
                case BucketMains: return "Mains";
                case BucketDesserts: return "Desserts";
                default: return "Drinks";
            }
        }

        private static int[] CoursesForBucket(int bucket)
        {
            switch (bucket)
            {
                case BucketStarters: return StarterCourses;
                case BucketMains: return MainCourses;
                case BucketDesserts: return DessertCourses;
                default: return new[] { 8, 9, 10, 11, 12, 13 };
            }
        }

        private static bool IsPending(OrderItem item) =>
            item.itemStatus == OrderStatus.Ordered ||
            item.itemStatus == OrderStatus.BeingPrepared;


        public List<BarKitchenViewModel> GetRunningOrders(BarKitchenStation station)
        {
            var (min, max) = Range(station);
            var orders = _repo.GetOpenOrdersWithItems(min, max);

            var result = new List<BarKitchenViewModel>();
            foreach (var order in orders)
            {
              
                if (!order.OrderItems.Any(IsPending))
                    continue;

                var vm = Map(order, new[]
                {
                    OrderStatus.Ordered,
                    OrderStatus.BeingPrepared,
                    OrderStatus.ReadyToBeServed
                });

                if (vm.Courses.Any())
                    result.Add(vm);
            }
            return result;
        }

        public List<BarKitchenViewModel> GetFinishedOrdersToday(BarKitchenStation station)
        {
            var (min, max) = Range(station);
            var orders = _repo.GetTodaysOrdersWithItems(min, max);

            var result = new List<BarKitchenViewModel>();
            foreach (var order in orders)
            {
                
                if (!order.OrderItems.Any()) continue;
                if (order.OrderItems.Any(IsPending)) continue;

                var vm = Map(order, new[]
                {
                    OrderStatus.ReadyToBeServed,
                    OrderStatus.Served
                });

                if (vm.Courses.Any())
                    result.Add(vm);
            }
            return result;
        }

        
        private static BarKitchenViewModel Map(OrderTable order, OrderStatus[] allowed)
        {
            var vm = new BarKitchenViewModel
            {
                TableOrderID = order.TableOrderID,
                TableNumber = order.TableNumber,
                CreatedAt = order.CreatedAt,
                OrderStatus = order.orderStatus
            };

            var items = order.OrderItems
                .Where(oi => allowed.Contains(oi.itemStatus))
                .Select(oi => new BarKitchenItemViewModel
                {
                    OrderItemID = oi.OrderItemID,
                    Name = oi.MenuItem?.Description ?? $"Item #{oi.MenuItemID}",
                    Quantity = oi.Quantity,
                    Comments = oi.Comments,
                    ItemStatus = oi.itemStatus,
                    CourseType = oi.MenuItem?.CourseType ?? 0
                });

            vm.Courses = items
                .GroupBy(i => BucketOf(i.CourseType))
                .OrderBy(g => g.Key == BucketDrinks ? 99 : g.Key) 
                .Select(g => new BarKitchenCourseGroup
                {
                    CourseBucket = g.Key,
                    CourseName = BucketName(g.Key),
                    Items = g.OrderBy(i => i.Name).ToList()
                })
                .ToList();

            return vm;
        }


        public void ChangeOrderStatus(int orderId, OrderStatus status, BarKitchenStation station)
        {
            var (min, max) = Range(station);
            _repo.UpdateOrderStatus(orderId, status, min, max);
        }

        public void ChangeOrderItemStatus(int orderItemId, OrderStatus status)
        {
            _repo.UpdateOrderItemStatus(orderItemId, status);
        }

        public void ChangeCourseStatus(int tableOrderId, int courseBucket, OrderStatus status)
        {
            int[] courses = CoursesForBucket(courseBucket);
            _repo.UpdateItemsStatusByCourseTypes(tableOrderId, courses, status);
        }
    }
}
