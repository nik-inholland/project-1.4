using WebApplication3.Models;

namespace WebApplication3.repo
{
    public interface IOrderRepository
    {
        OrderTable? GetById(int id);

        List<OrderTable> GetAll();

        List<OrderTable> GetRunningOrders();

        List<OrderTable> GetFinishedOrdersToday();

        List<PersonOrder> GetPersonOrdersByTableId(int tableOrderId);

        List<OrderItem> GetOrderItemsByOrderId(int tableOrderId);

        void UpdateOrderStatus(int orderId, int status);

        void UpdatePersonOrderStatus(int personOrderId, int status);

        void UpdateOrderItemStatus(int orderItemId, int status);

        void UpdateCourseStatus(int tableOrderId, int courseType, int status);
    }
}