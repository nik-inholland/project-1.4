using WebApplication3.Models;

namespace WebApplication3.Services.Interfaces
{
    public interface IOrderService
    {
        OrderTable GetOrder(int id);
        List<OrderTable> GetAllOrders();

        public List<OrderTable> GetRecentTableOrders(int count = 10);

        void ChangeOrderStatus(OrderTable order);

        void ChangePersonOrderStatus(PersonOrder personOrder);

        void MarkPersonAsServed(PersonOrder personOrder);
    }
}