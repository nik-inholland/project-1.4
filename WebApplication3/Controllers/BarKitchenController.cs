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
            return Kitchen();
        }

        public IActionResult Bar()
        {
            ViewBag.Mode = "Running";
            ViewBag.Station = "Bar";

            var orders = _service.GetRunningOrdersWithItems();

            return View("Index", orders);
        }

        public IActionResult Kitchen()
        {
            ViewBag.Mode = "Running";
            ViewBag.Station = "Kitchen";

            var orders = _service.GetRunningOrdersWithItems();

            return View("Index", orders);
        }

        public IActionResult Finished(string station = "Kitchen")
        {
            ViewBag.Mode = "Finished";
            ViewBag.Station = station;

            var orders = _service.GetFinishedOrdersTodayWithItems();

            return View("Index", orders);
        }

        [HttpPost]
        public IActionResult ChangeOrderItemStatus(
            int orderItemId,
            OrderStatus status,
            string station)
        {
            _service.ChangeOrderItemStatus(orderItemId, status);

            return RedirectToStation(station);
        }

        [HttpPost]
        public IActionResult ResetVisibleItemsStatus(
    List<int> orderItemIds,
    string station)
        {
            if (orderItemIds != null && orderItemIds.Count > 0)
            {
                _service.ResetVisibleItemsStatus(orderItemIds);
            }

            if (station == "Bar")
                return RedirectToAction("Bar");

            return RedirectToAction("Kitchen");
        }

        [HttpPost]
        public IActionResult ResetAllRunningItemsStatus(string station)
        {
            var orders = _service.GetRunningOrdersWithItems();

            List<int> allItemIds = new();

            foreach (var order in orders)
            {
                allItemIds.AddRange(
                    order.Drinks.Select(item => item.OrderItemID));

                allItemIds.AddRange(
                    order.Starters.Select(item => item.OrderItemID));

                allItemIds.AddRange(
                    order.Mains.Select(item => item.OrderItemID));

                allItemIds.AddRange(
                    order.Desserts.Select(item => item.OrderItemID));
            }

            _service.ResetVisibleItemsStatus(allItemIds);

            if (station == "Bar")
                return RedirectToAction("Bar");

            return RedirectToAction("Kitchen");
        }

        [HttpPost]
        public IActionResult ChangeMultipleOrderItemsStatus(
            List<int> orderItemIds,
            OrderStatus status,
            string station)
        {
            if (orderItemIds != null && orderItemIds.Count > 0)
            {
                _service.ChangeMultipleOrderItemsStatus(orderItemIds, status);
            }

            return RedirectToStation(station);
        }


        private IActionResult RedirectToStation(string station)
        {
            if (station == "Bar")
                return RedirectToAction("Bar");

            return RedirectToAction("Kitchen");
        }
    }
}