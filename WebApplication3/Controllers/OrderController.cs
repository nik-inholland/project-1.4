using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Services.Interfaces;
using WebApplication3.helper;

namespace WebApplication3.Controllers
{
    public class OrderController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;

        public OrderController(IMenuService menuService, IOrderService orderService)
        {
            _menuService = menuService;
            _orderService = orderService;
        }

        public IActionResult TakeOrder(int tableId)
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            var vm = new TakeOrderViewModel
            {
                TableID = tableId,
                MenuItems = _menuService.GetAllActive()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult SaveOrder(int tableId, List<CurrentOrderItem> currentOrder)
        {
            int employeeId = HttpContext.Session.GetInt32("EmployeeID") ?? 0;

            var orderItems = currentOrder.Select(x => new OrderItem
            {
                MenuItemID = x.MenuItemID,
                Quantity = x.Quantity,
                Comment = x.Comment
            }).ToList();

            _orderService.CreateNewOrder(tableId, employeeId, orderItems);

            // Decrease stock
            foreach (var item in currentOrder)
                _menuService.DecreaseStock(item.MenuItemID, item.Quantity);

            TempData["Success"] = "Order saved successfully!";
            return RedirectToAction("Index", "Table");
        }
    }
}