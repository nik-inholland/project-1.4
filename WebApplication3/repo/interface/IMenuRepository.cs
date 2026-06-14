using WebApplication3.Models;

namespace WebApplication3.repo.@interface
{
    public interface IMenuRepository
    {
        List<MenuItem> GetAllMenuItems();
        List<MenuItem> GetMenuItemsByCardType(string cardType); 
        List<MenuItem> GetMenuItemsByCategory(int category);    
        MenuItem GetMenuItemById(int menuItemID);
        void UpdateStock(int menuItemID, int quantityChange);   
    }
}