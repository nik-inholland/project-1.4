using WebApplication3.Models;
using WebApplication3.Models.ViewModels;

namespace WebApplication3.Services.Interfaces
{
    
    public enum BarKitchenStation
    {
        Kitchen,   
        Bar,       
        All       
    }

    public interface IBarKitchenService
    {
        List<BarKitchenViewModel> GetRunningOrders(BarKitchenStation station);
        List<BarKitchenViewModel> GetFinishedOrdersToday(BarKitchenStation station);

        void ChangeOrderStatus(int orderId, OrderStatus status, BarKitchenStation station);
        void ChangeOrderItemStatus(int orderItemId, OrderStatus status);
        void ChangeCourseStatus(int tableOrderId, int courseBucket, OrderStatus status);
    }
}
