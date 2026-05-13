using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.repo;
using WebApplication3.Repo.Folder_OrderItem;

namespace WebApplication3.Controllers
{
    public class Order_item_managment : Controller
    {
        private readonly Iorder_item_managment _repo;

        public Order_item_managment(Iorder_item_managment repo)
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