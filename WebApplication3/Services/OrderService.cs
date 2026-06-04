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

            order.PersonOrders =
                _repo.GetPersonOrdersByTableId(id);

            order.OrderItems =
                _repo.GetOrderItemsByOrderId(id);

            return order;
        }

        public List<OrderTable> GetAllOrders()
        {
            return _repo.GetAll();
        }

        public List<OrderTable> GetRunningOrders()
        {
            return _repo.GetRunningOrders();
        }

        public List<OrderTable> GetRunningOrdersWithItems()
        {
            var orders = _repo.GetRunningOrders();

            foreach (var order in orders)
            {
                order.OrderItems =
                    _repo.GetOrderItemsByOrderId(
                        order.TableOrderID);
            }

            return orders;
        }

        public List<OrderTable> GetFinishedOrdersTodayWithItems()
        {
            var orders = _repo.GetFinishedOrdersToday();
            foreach (var order in orders)
            {
                order.OrderItems =
                    _repo.GetOrderItemsByOrderId(
                        order.TableOrderID);
            }
            return orders;
        }

        public void ChangeOrderStatus(
            int orderId,
            OrderStatus status)
        {
            _repo.UpdateOrderStatus(
                orderId,
                (int)status);
        }

        public void ChangeOrderItemStatus(
            int orderItemId,
            OrderStatus status)
        {
            _repo.UpdateOrderItemStatus(
                orderItemId,
                (int)status);
        }

        public void ChangeCourseStatus(
            int tableOrderId,
            int courseType,
            OrderStatus status)
        {
            _repo.UpdateCourseStatus(
                tableOrderId,
                courseType,
                (int)status);
        }

        public void ChangePersonOrderStatus(
            int personOrderId,
            OrderStatus status)
        {
            _repo.UpdatePersonOrderStatus(
                personOrderId,
                (int)status);
        }

        public void MarkPersonAsServed(int personOrderId)
        {
            var orders = _repo.GetAll();

            PersonOrder target = null;

            foreach (var order in orders)
            {
                var persons =
                    _repo.GetPersonOrdersByTableId(
                        order.TableOrderID);

                target =
                    persons.FirstOrDefault(
                        p => p.PersonOrderID == personOrderId);

                if (target != null)
                    break;
            }

            if (target == null)
                return;

            if (target.OrderStatus != OrderStatus.ReadyToBeServed)
                return;

            _repo.UpdatePersonOrderStatus(
                personOrderId,
                (int)OrderStatus.Served);
        }
    }
}