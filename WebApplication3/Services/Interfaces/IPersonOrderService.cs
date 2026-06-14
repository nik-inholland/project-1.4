using WebApplication3.Models;

namespace WebApplication3.Services.Interfaces
{
    public interface IPersonOrderService
    {
        void Update(PersonOrder personOrder);
        List<OrderItem> GetOrderItemsByPersonOrderId(int personOrderID);
        List<PersonOrder> GetPersonOrdersByTable(OrderTable table);

    }
}
