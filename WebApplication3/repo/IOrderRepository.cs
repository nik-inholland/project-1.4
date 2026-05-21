using WebApplication3.Models;

namespace WebApplication3.repo
{
    public interface IOrderRepository
    {
        // GET
        OrderTable? GetById(int id);
        List<OrderTable> GetAll();
        List<PersonOrder> GetPersonOrdersByTableId(int tableOrderId);

        // UPDATE
        void UpdateOrderStatus(int orderId, int status);
        void UpdatePersonOrderStatus(int personOrderId, int status);
    }
}