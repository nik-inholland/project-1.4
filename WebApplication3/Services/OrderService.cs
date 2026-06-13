using WebApplication3.Models;
using WebApplication3.repo;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }
        public OrderTable GetOrder(int id)
        {
            var order = _repo.GetById(id);

            if (order == null)
                return null;

            order.PersonOrders = _repo.GetPersonOrdersByTableId(id);

            foreach (var person in order.PersonOrders)
            {
                person.OrderItems = _repo.GetOrderItemsByPersonOrderId(person.PersonOrderID);
            }

            return order;
        }

        public List<OrderTable> GetAllOrders()
        {
            var orders = _repo.GetAllTableOrders();
            foreach (var order in orders)
            {
                order.PersonOrders = _repo.GetPersonOrdersByTableId(order.TableOrderID);

                foreach (var person in order.PersonOrders)
                {
                    person.OrderItems = _repo.GetOrderItemsByPersonOrderId(person.PersonOrderID);
                }
            }

            return orders;
        }


        public void ChangeOrderStatus(OrderTable order)
        {
            _repo.UpdateOrderStatus(order);
        }

        public void ChangePersonOrderStatus(PersonOrder personOrder)
        {
            _repo.UpdatePersonOrderStatus(personOrder);
        }
        public void MarkPersonAsServed(PersonOrder personOrder)
        {
            personOrder.OrderStatus = OrderStatus.Served;

            _repo.UpdatePersonOrderStatus(personOrder);
        }

        public List<OrderTable> GetRecentTableOrders(int count = 10)
        {
            var orders = _repo.GetRecentTableOrders(count);
            foreach (var order in orders)
            {
                order.PersonOrders = _repo.GetPersonOrdersByTableId(order.TableOrderID);

                foreach (var person in order.PersonOrders)
                {
                    person.OrderItems = _repo.GetOrderItemsByPersonOrderId(person.PersonOrderID);
                }
            }

                
            return orders;
        }
    }
}