using WebApplication3.Models;

namespace WebApplication3.Services.Interfaces
{
    public interface IOrderService
    {
        OrderTable GetOrder(int id);
        List<OrderTable> GetAllOrders();

        void ChangeOrderStatus(int orderId, OrderStatus status);

        void ChangePersonOrderStatus(int personOrderId, OrderStatus status);

        void MarkPersonAsServed(int personOrderId);
    }
}