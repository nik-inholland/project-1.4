using WebApplication3.Models;
using WebApplication3.Models.ViewModels;

namespace WebApplication3.Services.Interfaces
{
    public interface ITableService
    {
        List<TableViewModel> GetAll();

        TableViewModel? GetById(int id);
        void ToggleTableStatus(int id);
        void Update(RestaurantTable table);
        TableStatus? GetTableStatus(int tableId);
    }
}

