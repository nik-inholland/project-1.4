using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Controllers
{
    [Authorize(Roles = "admin,barstaff,kitchenstaff,chef")]
    public class BarKitchenController : Controller
    {
        private readonly IBarKitchenService _service;

        // ---- CHANGE THESE if your Employee.employeeType values differ ----
        // Role claims are stored lower-cased (EmployeeService.SignInUser), keep lower-case.
        private const string KitchenRole = "kitchenstaff";
        private const string KitchenRoleAlt = "chef";   // some seed data uses this instead
        private const string BarRole = "barstaff";

        public BarKitchenController(IBarKitchenService service)
        {
            _service = service;
        }

        // Station forced by the employee's role.
        private BarKitchenStation RoleStation()
        {
            if (User.IsInRole(KitchenRole) || User.IsInRole(KitchenRoleAlt))
                return BarKitchenStation.Kitchen;
            if (User.IsInRole(BarRole))
                return BarKitchenStation.Bar;
            return BarKitchenStation.All; // admin / manager
        }

        // Admin/manager may pick a station via the Kitchen/Bar/All buttons.
        // Bar/kitchen employees are LOCKED to their own station (cannot see the other).
        private BarKitchenStation ResolveStation(string? requested)
        {
            var role = RoleStation();
            if (role != BarKitchenStation.All) return role;             // staff: locked
            if (Enum.TryParse<BarKitchenStation>(requested, true, out var chosen))
                return chosen;                                          // admin: chosen
            return BarKitchenStation.All;                              // admin default
        }

        // Running orders (auto food/drinks by role; admin can switch station).
        public IActionResult Index(string? station = null)
        {
            var st = ResolveStation(station);
            ViewBag.Mode = "Running";
            ViewBag.Station = st.ToString();
            ViewBag.CanChooseStation = RoleStation() == BarKitchenStation.All;
            return View(_service.GetRunningOrders(st));
        }

        // Finished orders of today (same view, reused).
        public IActionResult Finished(string? station = null)
        {
            var st = ResolveStation(station);
            ViewBag.Mode = "Finished";
            ViewBag.Station = st.ToString();
            ViewBag.CanChooseStation = RoleStation() == BarKitchenStation.All;
            return View("Index", _service.GetFinishedOrdersToday(st));
        }

        // Whole order: ordered -> being-prepared -> ready-to-be-served.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeOrderStatus(int orderId, OrderStatus status, string? station)
        {
            try
            {
                _service.ChangeOrderStatus(orderId, status, ResolveStation(station));
                TempData["Success"] = $"Order #{orderId} set to {status}.";
            }
            catch
            {
                TempData["Error"] = "Could not update the order status.";
            }
            return RedirectToAction(nameof(Index), new { station });
        }

        // A particular course (within a food order).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeCourseStatus(int tableOrderId, int courseType, OrderStatus status, string? station)
        {
            try
            {
                _service.ChangeCourseStatus(tableOrderId, courseType, status);
                TempData["Success"] = "Course status updated.";
            }
            catch
            {
                TempData["Error"] = "Could not update the course status.";
            }
            return RedirectToAction(nameof(Index), new { station });
        }

        // A particular order item.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeOrderItemStatus(int orderItemId, OrderStatus status, string? station)
        {
            try
            {
                _service.ChangeOrderItemStatus(orderItemId, status);
                TempData["Success"] = $"Item set to {status}.";
            }
            catch
            {
                TempData["Error"] = "Could not update the item status.";
            }
            return RedirectToAction(nameof(Index), new { station });
        }
    }
}
