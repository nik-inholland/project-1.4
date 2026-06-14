using WebApplication3.Models;

namespace WebApplication3.repo.@interface
{
    public interface IOrderRepository
    {
        OrderTable? GetById(int id);
        List<OrderTable> GetAllTableOrders();
        public List<OrderTable> GetRecentTableOrders(int count = 10);
        
        void Update(OrderTable order);
        void UpdateStatus(int orderId, OrderStatus status);
        void CloseOrder(int orderId, decimal totalPrice, DateTime closedAt);
        void ReopenOrder(int orderId);
    }
}