using WebApplication3.Models;
using WebApplication3.repo.@interface;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services
{
    public class PersonOrderService : IPersonOrderService
    {
        private readonly IPersonOrderRepository _repo;

        public PersonOrderService(IPersonOrderRepository repo)
        { 
            _repo = repo;
        }

        public List<OrderItem> GetOrderItemsByPersonOrderId(int personOrderID) => _repo.GetOrderItemsByPersonOrderId(personOrderID);

        public List<PersonOrder> GetPersonOrdersByTable(OrderTable table) => _repo.GetPersonOrdersByTable(table);

        public void Update(PersonOrder personOrder) => _repo.Update(personOrder);
    }
}