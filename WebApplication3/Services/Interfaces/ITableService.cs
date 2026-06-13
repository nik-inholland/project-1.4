using WebApplication3.Models;

namespace WebApplication3.Services.Interfaces
{
    public interface ITableService
    {
        
        List<RestaurantTable> GetAll();

        RestaurantTable? GetById(int id);

        void Update(RestaurantTable table);
    }
}

