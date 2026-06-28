using EllipticCurve.Utils;
using WebApplication3.Exceptions;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.repo;
using WebApplication3.repo.@interface;
using WebApplication3.Services.@interface;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IMenuItemService _menuItemService;
        private readonly ITableService _tableService;

        public OrderService(IOrderRepository orderRepo,
                            IMenuItemService menuItemService,
                            ITableService tableService)
        {
            _orderRepo = orderRepo;
            _menuItemService = menuItemService;
            _tableService = tableService;
        }

        public OrderTable? GetOrder(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return null;

            foreach (var item in order.OrderItems)
            {
                item.MenuItem = _menuItemService.GetMenuItemById(item.MenuItemID);
            }
            return order;
        }

        public OrderTable? GetOrderLight(int id)
        {
            var order = _orderRepo.GetById(id);
            return order;
        }

        public List<OrderTable> GetAllOrders(bool includeClosed = false)
        {
            var orders = _orderRepo.GetAllTableOrders();
            if (!includeClosed)
                orders = orders.Where(o => !o.IsClosed).ToList();
            return orders;
        }

        public List<OrderTable> GetRecentTableOrders(int count, bool showClosed, string? dateFilter = null)
        {
            return _orderRepo.GetRecentTableOrders(count, showClosed, dateFilter);
        }

        public void UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _orderRepo.GetById(orderId);
            if (order == null)
                throw new NotFoundException($"Order {orderId} not found");
            if (order.IsClosed)
                throw new InvalidOperationException("Cannot update a closed order");
            _orderRepo.UpdateStatus(orderId, newStatus);
        }

        public void CloseOrder(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null)
                throw new NotFoundException($"Order {id} not found");
            if (order.IsClosed)
                throw new InvalidOperationException("Order is already closed");

            decimal total = order.CalculateTotal();

            _orderRepo.CloseOrder(id, total, DateTime.Now);

            _tableService?.ToggleTableStatus(order.TableNumber);
        }

        public void ReopenOrder(int orderId)
        {
            var order = _orderRepo.GetById(orderId);
            if (order == null)
                throw new NotFoundException($"Order {orderId} not found");
            if (!order.IsClosed)
                throw new InvalidOperationException("Order is not closed");
            _orderRepo.ReopenOrder(orderId);
            _tableService?.ToggleTableStatus(order.TableNumber);
        }

        public void CancelOrder(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null)
                throw new NotFoundException($"Order {id} not found");
            if (order.IsClosed)
                throw new InvalidOperationException("Cannot cancel a closed order");
            if (order.orderStatus == OrderStatus.Served)
                throw new InvalidOperationException("Cannot cancel an order that has been served");

            order.orderStatus = OrderStatus.Cancelled;
            order.ClosedAt = DateTime.Now;
            _orderRepo.Update(order);
        }

        public void SaveOrder(OrderTable order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            if (order.OrderItems == null || !order.OrderItems.Any())
                throw new ArgumentException("Order must have at least one item");

            order.TotalPrice = order.CalculateTotal();
            order.orderStatus = OrderStatus.Ordered;
            order.CreatedAt = DateTime.Now;
            order.PaymentID = 0;

            _orderRepo.SaveOrder(order);
        }

        public bool IsOrderClosed(int id)
        {
            var order = _orderRepo.GetById(id);
            return order?.IsClosed ?? true;
        }

        public OrderTable? GetActiveOrderByTable(int tableNumber)
        {
            var allOrders = _orderRepo.GetAllTableOrders();
            var activeOrder = allOrders
                .Where(o => o.TableNumber == tableNumber && o.ClosedAt == null)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            if (activeOrder == null) return null;

            return GetOrder(activeOrder.TableOrderID);
        }

        public List<OrderTable> GetActiveOrders(int? limit = null)
        {
            return _orderRepo.GetOrdersByClosedState(false, limit);
        }

        public List<OrderTable> GetClosedOrders(int? limit = null)
        {
            return _orderRepo.GetOrdersByClosedState(true, limit);
        }

        public OrderDetailViewModel? GetOrderDetails(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return null;

            foreach (var item in order.OrderItems)
            {
                item.MenuItem ??= _menuItemService.GetMenuItemById(item.MenuItemID);
            }

            var items = order.OrderItems.Select(oi =>
            {
                string categoryName = oi.MenuItem != null
                    ? ((ItemCategory)oi.MenuItem.CourseType).ToString()
                    : "Unknown";

                return new WebApplication3.Models.ViewModels.OrderItemDetailViewModel
                {
                    OrderItemID = oi.OrderItemID,
                    Name = oi.MenuItem?.Description ?? $"Item #{oi.MenuItemID}",
                    Quantity = oi.Quantity,
                    Price = oi.MenuItem?.Price ?? 0,
                    Category = categoryName,
                    Comments = oi.Comments,
                    ItemStatus = oi.itemStatus
                };
            }).ToList();

            decimal totalPrice = items.Sum(i => i.Quantity * i.Price);

            return new OrderDetailViewModel
            {
                TableOrderID = order.TableOrderID,
                TableNumber = order.TableNumber,
                Status = order.orderStatus,
                CreatedAt = order.CreatedAt,
                IsClosed = order.IsClosed,
                ClosedAt = order.ClosedAt,
                TotalPrice = totalPrice,
                Items = items
            };
        }

        public List<OrderItem> GetOrderItems(int orderId)
        {
            return _orderRepo.GetOrderItems(orderId);
        }

        public void ToggleItemStatus(int orderId)
        {
            OrderItem item = _orderRepo.GetOrderItemById(orderId);
            if (item == null)
                throw new NotFoundException($"Order {orderId} not found");

            if (item.itemStatus == OrderStatus.ReadyToBeServed)
            {
                item.itemStatus = OrderStatus.Served;
                _orderRepo.UpdateOrderItemStatus(orderId, OrderStatus.Served);
            }
            else if (item.itemStatus == OrderStatus.Served)
            {
                item.itemStatus = OrderStatus.ReadyToBeServed;
                _orderRepo.UpdateOrderItemStatus(orderId, OrderStatus.ReadyToBeServed);
            }
            else
                throw new InvalidOperationException(
                    $"Order item status '{item.itemStatus}' cannot be toggled. Only ReadyToBeServed and Served are allowed.");
        }

        public void ToggleOrderStatus(int orderId)
        {
            var order = _orderRepo.GetById(orderId);
            if (order == null)
                throw new NotFoundException($"Order {orderId} not found");

            OrderStatus newStatus;
            if (order.orderStatus == OrderStatus.ReadyToBeServed)
                newStatus = OrderStatus.Served;
            else if (order.orderStatus == OrderStatus.Served)
                newStatus = OrderStatus.ReadyToBeServed;
            else
                throw new InvalidOperationException(
                    $"Order status '{order.orderStatus}' cannot be toggled. Only ReadyToBeServed and Served are allowed.");

            _orderRepo.UpdateStatus(orderId, newStatus);
        }
    }
}