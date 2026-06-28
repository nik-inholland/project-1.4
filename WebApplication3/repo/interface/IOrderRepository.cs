using WebApplication3.Models;

public interface IOrderRepository
{
    OrderTable? GetById(int id);
    List<OrderTable> GetAllTableOrders();
    List<OrderTable> GetRecentTableOrders(int count, bool showClosed, string? dateFilter =  null);
    void Update(OrderTable order);
    void UpdateStatus(int orderId, OrderStatus status);
    void UpdateOrderItemStatus(int orderId, OrderStatus status);
    void CloseOrder(int orderId, decimal totalPrice, DateTime closedAt);
    void ReopenOrder(int orderId);
    void SaveOrder(OrderTable order);
    List<OrderTable> GetOrdersByClosedState(bool closed, int? limit = null);
    List<OrderItem> GetOrderItems(int orderId);
    public OrderItem GetOrderItemById(int orderId);
}