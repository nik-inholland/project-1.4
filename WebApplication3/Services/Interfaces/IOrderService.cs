using WebApplication3.Models;
using WebApplication3.Models.ViewModels;

namespace WebApplication3.Services.Interfaces
{
    public interface IOrderService
    {
        OrderDetailViewModel? GetOrder(int id);
        OrderTable? GetOrderLight(int id);
        List<OrderTable> GetAllOrders(bool includeClosed = false);
        List<OrderListViewModel> GetRecentTableOrders(int count = 10, bool includeClosed = false);
        void UpdateOrderStatus(int orderId, OrderStatus newStatus);
        void CloseOrder(int id);
        void Update(OrderTable order);
        bool IsOrderClosed(int id);
        void ReopenOrder(int orderId);
        void SaveOrder(int tableNumber, List<OrderItem> orderItems);
    }
}