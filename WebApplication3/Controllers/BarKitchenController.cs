using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Controllers
{
    public class BarKitchenController : Controller
    {
        private readonly IOrderService _service;

        public BarKitchenController(IOrderService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            ViewBag.Mode = "Running";

            var runningOrders =
                _service.GetRunningOrdersWithItems();

            return View(runningOrders);
        }

        [HttpPost]
        public IActionResult ChangeOrderStatus(
            int orderId,
            OrderStatus status)
        {
            _service.ChangeOrderStatus(
                orderId,
                status);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangeOrderItemStatus(
            int orderItemId,
            OrderStatus status)
        {
            _service.ChangeOrderItemStatus(
                orderItemId,
                status);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangeCourseStatus(
            int tableOrderId,
            int courseType,
            OrderStatus status)
        {
            _service.ChangeCourseStatus(
                tableOrderId,
                courseType,
                status);

            return RedirectToAction("Index");
        }

        public IActionResult Finished()
        {
            ViewBag.Mode = "Finished";

            var finishedOrders =
                _service.GetFinishedOrdersTodayWithItems();

            return View("Index", finishedOrders);
        }
    }
}