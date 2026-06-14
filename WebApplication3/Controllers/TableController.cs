using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Exceptions;
using WebApplication3.Services;
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

        [Authorize]
        public IActionResult Index()
        {
            var tables = _repository.GetAll();
            return View(tables);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            try
            {
                _repository.ToggleTableStatus(id);
                TempData["Success"] = "Table status updated.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch
            {
                TempData["Error"] = "Unable to update table status.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}