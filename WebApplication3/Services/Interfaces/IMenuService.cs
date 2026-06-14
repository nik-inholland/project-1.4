using WebApplication3.Models;
using WebApplication3.Models.ViewModels;

namespace WebApplication3.Services.Interfaces
{
    public interface IMenuService
    {
        List<MenuItem> GetAllMenuItems();
        List<MenuItem> GetMenuItemsByCard(string cardType);    
        List<MenuItem> GetMenuItemsByCourseType(int courseType);

        MenuItem? GetMenuItemById(int menuItemID);

       
        bool CanAddToOrder(int menuItemID);
        void DecreaseStockForOrder(List<OrderItem> items);

       
        MenuViewModel GetMenuViewModel(string filterCard = "All", int filterCourse = 0);
    }
}