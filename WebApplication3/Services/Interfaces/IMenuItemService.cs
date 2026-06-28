using WebApplication3.Models;

namespace WebApplication3.Services.@interface
{
    public interface IMenuItemService
    {
        IEnumerable<MenuItem> GetAllMenuItems();
        MenuItem? GetMenuItemById(int id);
        void AddMenuItem(MenuItem item);
        void UpdateMenuItem(MenuItem item);
        void DeleteMenuItem(int id);
        void RefreshCache();
    }
}