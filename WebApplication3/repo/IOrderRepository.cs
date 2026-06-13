using WebApplication3.Models;

namespace WebApplication3.repo
{
    public interface IOrderRepository
    {
        OrderTable? GetById(int id);
        List<OrderTable> GetAllTableOrders();
        public List<OrderTable> GetRecentTableOrders(int count = 10);
        List<PersonOrder> GetPersonOrdersByTableId(int tableOrderId);

        void UpdateOrderStatus(OrderTable order);
        void UpdatePersonOrderStatus(PersonOrder po);

        List<OrderItem> GetOrderItemsByPersonOrderId(int personOrderId);
    }
}