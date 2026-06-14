using WebApplication3.Models;

namespace WebApplication3.repo.@interface
{
    public interface ITableRepository
    {
        List<RestaurantTable> GetAll();

        RestaurantTable? GetById(int id);

        void Update(RestaurantTable table);
    }
}