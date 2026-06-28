using WebApplication3.Models;

namespace WebApplication3.repo.@interface
{
    public interface IMenuItemRepository
    {
        IEnumerable<MenuItem> GetAll();
        MenuItem? GetById(int id);
        void Add(MenuItem item);
        void Update(MenuItem item);
        void Delete(int id);
        IEnumerable<MenuItem> GetByCourseType(int courseType);
        IEnumerable<MenuItem> GetInStock();
    }
}
