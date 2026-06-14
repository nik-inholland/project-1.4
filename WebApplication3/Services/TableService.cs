using WebApplication3.Exceptions;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.repo.@interface;
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

        public List<TableViewModel> GetAll()
        {
            return _tableRepository.GetAll().Select(t => new TableViewModel
            {
                TableID = t.TableID,
                Occupied = t.Occupied
            }).ToList();
        }

        public TableViewModel? GetById(int id)
        {
            var table = _tableRepository.GetById(id);
            if (table == null) return null;
            return new TableViewModel { TableID = table.TableID, Occupied = table.Occupied };
        }

        public void Update(RestaurantTable table)
        {
            _tableRepository.Update(table);
        }

        public void ToggleTableStatus(int id)
        {
            var table = _tableRepository.GetById(id);
            if (table == null) throw new NotFoundException($"Table {id} not found");
            table.Occupied = table.Occupied == TableStatus.Free ? TableStatus.Occupied : TableStatus.Free;
            _tableRepository.Update(table);
        }

        public TableStatus? GetTableStatus(int tableId)
        {
            var table = _tableRepository.GetById(tableId);
            return table?.Occupied;
        }
    }
}