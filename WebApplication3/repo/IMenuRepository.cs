using WebApplication3.Models;

namespace WebApplication3.repo
{
    public interface IMenuRepository
    {
        List<MenuItem> GetAllActive();
        List<MenuItem> GetFiltered(string cardType, string category);
        MenuItem? GetById(int id);
        void DecreaseStock(int menuItemId, int quantity);
    }
}