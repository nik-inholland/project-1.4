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

        void UpdateOrderItemStatus(int orderItemId, int status);

    }
}