using WebApplication3.Models;

namespace WebApplication3.Services.Interfaces
{
    public interface IOrderService
    {
        OrderTable GetOrder(int id);

        List<OrderTable> GetAllOrders();

        List<OrderTable> GetRunningOrders();

        List<OrderTable> GetRunningOrdersWithItems();

        List<OrderTable> GetFinishedOrdersTodayWithItems();

        void ChangeOrderStatus(int orderId, OrderStatus status);

        void ChangeOrderItemStatus(int orderItemId, OrderStatus status);

        void ChangeCourseStatus(
            int tableOrderId,
            int courseType,
            OrderStatus status);

        void ChangePersonOrderStatus(
            int personOrderId,
            OrderStatus status);

        void MarkPersonAsServed(int personOrderId);
    }
}