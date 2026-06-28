using WebApplication3.Exceptions;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.repo.@interface;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly ILogger<MenuService> _logger;

        public MenuService(IMenuRepository menuRepository, ILogger<MenuService> logger)
        {
            _menuRepository = menuRepository;
            _logger = logger;
        }

        public List<MenuItem> GetAllMenuItems() => _menuRepository.GetAllMenuItems();

        public List<MenuItem> GetMenuItemsByCard(string cardType)
        {
            if (string.IsNullOrEmpty(cardType) || cardType == "All")
                return GetAllMenuItems();
            return _menuRepository.GetMenuItemsByCardType(cardType);
        }

        public List<MenuItem> GetMenuItemsByCourseType(int courseType)
        {
            return _menuRepository.GetMenuItemsByCategory(courseType);
        }

        public MenuItem? GetMenuItemById(int menuItemID)
        {
            return _menuRepository.GetMenuItemById(menuItemID);
        }

       
        public bool CanAddToOrder(int menuItemID)
        {
            var item = GetMenuItemById(menuItemID);
            return item != null && !item.IsOutOfStock();
        }

        public void DecreaseStockForOrder(List<OrderItem> items)
        {
            foreach (var item in items)
            {
                _menuRepository.UpdateStock(item.MenuItemID, -item.Quantity);
            }
        }

        public MenuViewModel GetMenuViewModel(string filterCard = "All", int filterCourse = 0)
        {
            var menuItems = GetMenuItemsByCard(filterCard);

            if (filterCourse > 0)
            {
                menuItems = menuItems.Where(m => m.CourseType == filterCourse).ToList();
            }

            return new MenuViewModel
            {
                MenuItems = menuItems,
                SelectedCard = filterCard,
                SelectedCourseType = filterCourse
            };
        }
    }
}