using WebApplication3.Models;

namespace WebApplication3.repo.@interface
{
    public interface IPersonOrderRepository
    {
        List<OrderItem> GetOrderItemsByPersonOrderId(int personOrderID);
        List<PersonOrder> GetPersonOrdersByTable(OrderTable table);
        void Update(PersonOrder po);
    }
}
