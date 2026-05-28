using WebApplication3.Models;
using WebApplication3.repo;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _repo;

        public MenuService(IMenuRepository repo)
        {
            _repo = repo;
        }

        public List<MenuItem> GetAllActive() => _repo.GetAllActive();
        public List<MenuItem> GetFiltered(string cardType, string category) => _repo.GetFiltered(cardType, category);
        public MenuItem? GetById(int id) => _repo.GetById(id);

        public void DecreaseStock(int menuItemId, int quantity)
        {
            _repo.DecreaseStock(menuItemId, quantity);
        }
    }
}