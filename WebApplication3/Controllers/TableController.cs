using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Controllers
{
    public class TableController : Controller
    {
        private readonly ITableService _repository;

        public TableController(ITableService repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var tables = _repository.GetAll();
            return View(tables);
        }

        public IActionResult ToggleStatus(int id)
        {
            var table = _repository.GetById(id);

            if (table == null)
                return NotFound();

            table.Occupied =
                table.Occupied == TableStatus.Free
                ? TableStatus.Occupied
                : TableStatus.Free;

            _repository.Update(table);

            return RedirectToAction(nameof(Index));
        }
    }
}