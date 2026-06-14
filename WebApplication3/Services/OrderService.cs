using WebApplication3.Exceptions;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.repo.@interface;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IPersonOrderService _personOrderService;
        private readonly ITableService _tableService;

        public OrderService(
            IOrderRepository repo,
            IPersonOrderService personOrderService,
            ITableService tableService)
        {
            _repo = repo;
            _personOrderService = personOrderService;
            _tableService = tableService;
        }

        public OrderDetailViewModel? GetOrder(int id)
        {
            var order = _repo.GetById(id);
            if (order == null) return null;
            order.PersonOrders = _personOrderService.GetPersonOrdersByTable(order);
            foreach (var person in order.PersonOrders)
            {
                person.OrderItems = _personOrderService.GetOrderItemsByPersonOrderId(person.PersonOrderID);
                person.TotalPrice = person.OrderItems.Sum(i => (decimal)i.PricePerItem * i.Quantity);
                _personOrderService.Update(person);
            }
            order.TotalPrice = order.PersonOrders.Sum(p => p.TotalPrice);
            _repo.Update(order);
            return MapToDetailViewModel(order);
        }

        public OrderTable? GetOrderLight(int id)
        {
            return _repo.GetById(id);
        }

        public List<OrderTable> GetAllOrders(bool includeClosed = false)
        {
            var orders = _repo.GetAllTableOrders();
            foreach (var order in orders)
            {
                order.PersonOrders = _personOrderService.GetPersonOrdersByTable(order);
            }
            if (!includeClosed)
            {
                orders = orders.Where(o => o.ClosedAt == null).ToList();
            }
            return orders;
        }

        public List<OrderListViewModel> GetRecentTableOrders(int count = 10, bool includeClosed = false)
        {
            var orders = _repo.GetRecentTableOrders(count);
            foreach (var order in orders)
                order.PersonOrders = _personOrderService.GetPersonOrdersByTable(order);
            if (!includeClosed)
                orders = orders.Where(o => o.ClosedAt == null).ToList();
            return MapToListViewModel(orders);
        }

        public void UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _repo.GetById(orderId);
            if (order == null)
                throw new NotFoundException($"Order {orderId} not found");
            if (order.ClosedAt != null)
                throw new InvalidOperationException("Cannot update a closed order");
            _repo.UpdateStatus(orderId, newStatus);
        }

        public void CloseOrder(int id)
        {
            var order = _repo.GetById(id);
            if (order == null)
                throw new NotFoundException($"Order {id} not found");
            if (order.ClosedAt != null)
                throw new InvalidOperationException("Order is already closed");
            order.PersonOrders = _personOrderService.GetPersonOrdersByTable(order);
            foreach (var person in order.PersonOrders)
            {
                person.OrderItems = _personOrderService.GetOrderItemsByPersonOrderId(person.PersonOrderID);
                person.TotalPrice = person.OrderItems.Sum(i => (decimal)i.PricePerItem * i.Quantity);
                _personOrderService.Update(person);
            }
            decimal totalPrice = order.PersonOrders.Sum(p => p.TotalPrice);
            _repo.CloseOrder(id, totalPrice, DateTime.Now);
            var table = _tableService.GetById(order.TableNumber);
            if (table != null && table.Occupied == TableStatus.Occupied)
            {
                _tableService.ToggleTableStatus(order.TableNumber);
            }
        }

        public void ReopenOrder(int id)
        {
            var order = _repo.GetById(id);
            if (order == null)
                throw new NotFoundException($"Order {id} not found");
            if (order.ClosedAt == null)
                throw new InvalidOperationException("Order is not closed");
            _repo.ReopenOrder(id);
            var table = _tableService.GetById(order.TableNumber);
            if (table != null && table.Occupied == TableStatus.Free)
            {
                _tableService.ToggleTableStatus(order.TableNumber);
            }
        }

        public void CancelOrder(int id)
        {
            var order = _repo.GetById(id);
            if (order == null)
                throw new NotFoundException($"Order {id} not found");
            if (order.ClosedAt != null)
                throw new InvalidOperationException("Cannot cancel a closed order");
            if ((OrderStatus)order.OrderStatus == OrderStatus.Served)
                throw new InvalidOperationException("Cannot cancel an order that has been served");
            order.OrderStatus = OrderStatus.Cancelled;
            order.ClosedAt = DateTime.Now;
            _repo.Update(order);
        }

        public void Update(OrderTable order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            _repo.Update(order);
        }

        public bool IsOrderClosed(int id)
        {
            var order = _repo.GetById(id);
            return order?.ClosedAt != null;
        }

        public void SaveOrder(int tableNumber, List<OrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
                throw new ArgumentException("Cannot save empty order");

            try
            {
                var newOrder = new OrderTable
                {
                    TableNumber = tableNumber,
                    TotalPrice = orderItems.Sum(i => i.PricePerItem * i.Quantity),
                    OrderStatus = OrderStatus.Ordered,
                    CreatedAt = DateTime.Now
                };

                _repo.SaveOrder(newOrder, orderItems);   

                
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save order for table {tableNumber}", ex);
            }
        }

        private OrderDetailViewModel MapToDetailViewModel(OrderTable order)
        {
            return new OrderDetailViewModel
            {
                TableOrderID = order.TableOrderID,
                TableNumber = order.TableNumber,
                Status = (OrderStatus)order.OrderStatus,
                CreatedAt = order.CreatedAt,
                TotalPrice = order.TotalPrice,
                IsClosed = order.ClosedAt != null,
                ClosedAt = order.ClosedAt,
                PersonOrders = order.PersonOrders.Select(p => new PersonOrderViewModel
                {
                    PersonOrderID = p.PersonOrderID,
                    PersonName = p.PersonName,
                    Status = (OrderStatus)p.OrderStatus,
                    TotalPrice = p.TotalPrice,
                    Items = p.OrderItems.Select(i => new OrderItemViewModel
                    {
                        Name = i.ItemName,
                        Comments = i.Comments,
                        Price = (double)i.PricePerItem,
                        Quantity = i.Quantity,
                        Category = i.Category ??0
                    }).ToList()
                }).ToList()
            };
        }

        private List<OrderListViewModel> MapToListViewModel(List<OrderTable> orders)
        {
            return orders.Select(o => new OrderListViewModel
            {
                TableOrderID = o.TableOrderID,
                TableNumber = o.TableNumber,
                Status = (OrderStatus)o.OrderStatus,
                CreatedAt = o.CreatedAt,
                PersonCount = o.PersonOrders?.Count ?? 0,
                IsClosed = o.ClosedAt != null
            }).ToList();
        }
    }
}