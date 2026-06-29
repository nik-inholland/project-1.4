using WebApplication3.Models;

namespace WebApplication3.Services.Interfaces
{
    public interface IOrderService
    {
        OrderTable? GetOrder(int id);

        List<OrderTable> GetAllOrders();

        List<OrderTable> GetRunningOrders();

        List<OrderTable> GetRunningOrdersWithItems();

        List<OrderTable> GetFinishedOrdersTodayWithItems();

        void ChangeOrderItemStatus(
            int orderItemId,
            OrderStatus status);

        void ChangeMultipleOrderItemsStatus(
     List<int> orderItemIds,
     OrderStatus status); 

        void ResetVisibleItemsStatus(
            List<int> orderItemIds);


    }
}