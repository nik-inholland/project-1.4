using WebApplication3.Models;
using WebApplication3.repo;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;

        public TableService(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        public List<RestaurantTable> GetAll()
        {
            return _tableRepository.GetAll();
        }

        public RestaurantTable? GetById(int id)
        {
            return _tableRepository.GetById(id);
        }

        public void Update(RestaurantTable table)
        {
            _tableRepository.Update(table);
        }
    }
}