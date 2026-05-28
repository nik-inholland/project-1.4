using WebApplication3.Models;

namespace WebApplication3.repo
{
    public interface Iorder_item_managment
    {
        List<OrderItem> GetAll();

        OrderItem? GetById(int id);

        void Create(OrderItem orderItem);

        void Update(OrderItem orderItem);

        void Delete(int id);
    }
}