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

        public OrderTable? GetOrder(int id)
        {
            var order = _repo.GetById(id);

            if (order == null)
            {
                return null;
            }

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

            List<OrderTable> finishedOrders = new();

            foreach (var order in orders)
            {
                var items =
                    _repo.GetOrderItemsByOrderId(order.TableOrderID);

                order.OrderItems =
                    items
                    .Where(item => item.ItemStatus == OrderStatus.ReadyToBeServed)
                    .ToList();

                if (order.OrderItems.Any())
                {
                    finishedOrders.Add(order);
                }
            }

            return finishedOrders;
        }

        public void ChangeOrderItemStatus(
            int orderItemId,
            OrderStatus status)
        {
            _repo.UpdateOrderItemStatus(
                orderItemId,
                (int)status);
        }

        public void ResetVisibleItemsStatus(
     List<int> orderItemIds)
        {
            foreach (int orderItemId in orderItemIds)
            {
                _repo.UpdateOrderItemStatus(
                    orderItemId,
                    (int)OrderStatus.Ordered);
            }
        }

        public void ChangeMultipleOrderItemsStatus(
            List<int> orderItemIds,
            OrderStatus status)
        {
            foreach (int orderItemId in orderItemIds)
            {
                _repo.UpdateOrderItemStatus(
                    orderItemId,
                    (int)status);
            }
        }
    }
}