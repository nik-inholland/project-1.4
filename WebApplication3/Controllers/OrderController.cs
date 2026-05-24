using Microsoft.AspNetCore.Mvc;
using WebApplication3.helper;
using WebApplication3.Models;
using WebApplication3.Services;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var role = RoleHelper.GetRole(HttpContext);

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            var orders = _service.GetAllOrders();
            return View(orders);
        }
        public IActionResult Details(int id)
        {

            if (!RoleHelper.CanEditOrders(HttpContext)) 
            {
                return RedirectToAction("Index", "Home");
            }

            var order = _service.GetOrder(id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        public IActionResult ChangeOrderStatus(int orderId, OrderStatus status)
        {

            _service.ChangeOrderStatus(orderId, status);
            return RedirectToAction("Details", new { id = orderId });
        }

        [HttpPost]
        public IActionResult ServePerson(int personOrderId, int orderId)
        {
            _service.MarkPersonAsServed(personOrderId);
            return RedirectToAction("Details", new { id = orderId });
        }
    }
}