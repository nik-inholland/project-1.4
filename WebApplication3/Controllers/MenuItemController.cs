using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.repo;

namespace WebApplication3.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly Iorder_item_managment _repo;

        public MenuItemController(Iorder_item_managment repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            List<OrderItem> items = _repo.GetAll();

            return View(items);
        }
    }
}