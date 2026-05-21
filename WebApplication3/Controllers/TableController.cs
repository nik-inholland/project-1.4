using Microsoft.AspNetCore.Mvc;
using WebApplication3.helper;
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
            var role = RoleHelper.GetRole(HttpContext);

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            var tables = _repository.GetAll();
            return View(tables);
        }

        public IActionResult SetStatus(int id, TableStatus status)
        {
            var role = RoleHelper.GetRole(HttpContext);


            RestaurantTable? table = _repository.GetById(id);

            if (table == null)
                return NotFound();

            table.Occupied = status;

            _repository.Update(table);

            return RedirectToAction(nameof(Index));
        }
    }
}