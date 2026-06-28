using WebApplication3.Models;
using WebApplication3.Models.ViewModels;

namespace WebApplication3.Services.@interface
{
    public interface IOrderService
    {
        OrderTable? GetOrder(int id);
        OrderTable? GetOrderLight(int id);
        List<OrderTable> GetAllOrders(bool includeClosed = false);
        List<OrderTable> GetRecentTableOrders(int count = 10, bool includeClosed = false, string? dateFilter = null);
        void UpdateOrderStatus(int orderId, OrderStatus newStatus);
        void CloseOrder(int id);
        void ReopenOrder(int orderId);
        void CancelOrder(int id);
        void SaveOrder(OrderTable order);
        bool IsOrderClosed(int id);
        OrderTable? GetActiveOrderByTable(int tableNumber);
        void ToggleOrderStatus(int orderId);
        void ToggleItemStatus(int orderId);
        List<OrderTable> GetActiveOrders(int? limit = null);
        List<OrderTable> GetClosedOrders(int? limit = null);
        OrderDetailViewModel? GetOrderDetails(int id);
        List<OrderItem> GetOrderItems(int orderId);

    }
}