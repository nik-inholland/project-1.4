using WebApplication3.Models;

namespace WebApplication3.repo.@interface
{
    // course_type ranges (see MenuRepository): food = 1..7, drinks = 8..13.
    // Kitchen passes (1,7), Bar passes (8,13), admin passes (1,13).
    public interface IBarKitchenRepository
    {
        // All not-closed orders with their station items (any status). Oldest first.
        List<OrderTable> GetOpenOrdersWithItems(int minCourse, int maxCourse);

        // All of today's orders with their station items. Newest first.
        List<OrderTable> GetTodaysOrdersWithItems(int minCourse, int maxCourse);

        // Set the order header status AND only this station's items.
        void UpdateOrderStatus(int orderId, OrderStatus status, int minCourse, int maxCourse);

        // Set the status of a single order item.
        void UpdateOrderItemStatus(int orderItemId, OrderStatus status);

        // Set the status of every item in the order whose course_type is in the set.
        void UpdateItemsStatusByCourseTypes(int orderId, int[] courseTypes, OrderStatus status);
    }
}
