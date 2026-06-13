using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [Authorize]
        public IActionResult Index()
        {

            var orders = _service.GetAllOrders()
                                 .OrderBy(o => o.TableOrderID)
                                 .ToList();

            return View(orders);
        }

        [Authorize(Roles = "admin, waiter, kitchenstaff, barstaff")]
        public IActionResult Details(int id)
        {

            var order = _service.GetOrder(id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [Authorize(Roles = "admin, waiter, chef, barstaff")]
        [HttpPost]
        public IActionResult ChangeOrderStatus(OrderTable order)
        {

            _service.ChangeOrderStatus(order);
            return RedirectToAction("Details", new { id = order.TableOrderID });
        }

        [Authorize(Roles = "admin, waiter, kitchenstaff, barstaff")]
        [HttpPost]
        public IActionResult ChangePersonOrderStatus(PersonOrder personOrder)
        {
            _service.ChangePersonOrderStatus(personOrder);
            return RedirectToAction("Details", new { id = personOrder.TableOrderID });
        }

        [Authorize(Roles = "admin, waiter, chef, barstaff")]
        [HttpPost]
        public IActionResult ServePerson(PersonOrder personOrder)
        {
            _service.MarkPersonAsServed(personOrder);
            return RedirectToAction("Details", new { id = personOrder.TableOrderID });
        }
    }
}