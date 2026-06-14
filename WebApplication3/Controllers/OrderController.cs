using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.Services.Interfaces;
using WebApplication3.Exceptions;
using WebApplication3.Helpers;
using System.Linq;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPersonOrderService _personOrderService;
        private readonly IMenuService _menuService;

        public OrderController(
            IOrderService orderService,
            IPersonOrderService personOrderService,
            IMenuService menuService)
        {
            _orderService = orderService;
            _personOrderService = personOrderService;
            _menuService = menuService;
        }

        [Authorize]
        public IActionResult Index(bool showClosed = false, string? dateFilter = null)
        {
            try
            {
                var orders = _orderService.GetRecentTableOrders(10, showClosed);
                if (!string.IsNullOrEmpty(dateFilter) && DateTime.TryParse(dateFilter, out DateTime filterDate))
                    orders = orders.Where(o => o.CreatedAt.Date == filterDate.Date).ToList();
                ViewBag.ShowClosed = showClosed;
                ViewBag.DateFilter = dateFilter;
                return View(orders);
            }
            catch
            {
                TempData["Error"] = "Unable to load orders.";
                return View(new List<OrderListViewModel>());
            }
        }

        [Authorize(Roles = "admin, waiter, kitchenstaff, barstaff")]
        public IActionResult Details(int id)
        {
            try
            {
                var order = _orderService.GetOrder(id);
                if (order == null) return NotFound();
                return View(order);
            }
            catch
            {
                TempData["Error"] = "Unable to load order details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter, chef, barstaff")]
        public IActionResult ChangeOrderStatus(OrderTable order)
        {
            try
            {
                _orderService.UpdateOrderStatus(order.TableOrderID, order.OrderStatus);
                TempData["Success"] = "Order status updated.";
            }
            catch (NotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            catch { TempData["Error"] = "Unable to update order status."; }
            return RedirectToAction(nameof(Details), new { id = order.TableOrderID });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter, kitchenstaff, barstaff")]
        public IActionResult ChangePersonOrderStatus(int PersonOrderID, int Status, int TableOrderID)
        {
            try
            {
                var personOrder = new PersonOrder
                {
                    PersonOrderID = PersonOrderID,
                    OrderStatus = (OrderStatus)Status,
                    TableOrderID = TableOrderID
                };
                _personOrderService.Update(personOrder);
                TempData["Success"] = "Person order updated.";
            }
            catch { TempData["Error"] = "Unable to update person order."; }
            return RedirectToAction(nameof(Details), new { id = TableOrderID });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter, chef, barstaff")]
        public IActionResult CloseOrder(int id)
        {
            try { _orderService.CloseOrder(id); TempData["Success"] = "Order closed."; }
            catch (NotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            catch { TempData["Error"] = "Unable to close order."; }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter, chef, barstaff")]
        public IActionResult ReopenOrder(int id)
        {
            try { _orderService.ReopenOrder(id); TempData["Success"] = "Order reopened."; }
            catch (NotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            catch { TempData["Error"] = "Unable to reopen order."; }
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "admin, waiter")]
        public IActionResult Menu(string cardType = "All", int courseType = 0)
        {
            var vm = _menuService.GetMenuViewModel(cardType, courseType);
            return View(vm);
        }

        [Authorize(Roles = "admin, waiter")]
        public IActionResult TakeOrder(int tableNumber, string selectedCard = "All", int selectedCourse = 0)
        {
            try
            {
                var vm = HttpContext.Session.GetTakeOrderViewModel(tableNumber);
                vm.MenuItems = _menuService.GetAllMenuItems();
                ViewBag.SelectedCard = selectedCard;
                ViewBag.SelectedCourse = selectedCourse;
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading menu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter")]
        public IActionResult AddToOrder(int tableNumber, int menuItemID, int quantity = 1, string comments = "")
        {
            if (!_menuService.CanAddToOrder(menuItemID))
            {
                TempData["Error"] = "This item is currently out of stock!";
                return RedirectToAction(nameof(TakeOrder), new { tableNumber });
            }
            var menuItem = _menuService.GetMenuItemById(menuItemID);
            if (menuItem == null) return NotFound();
            var vm = HttpContext.Session.GetTakeOrderViewModel(tableNumber);
            vm.AddItem(menuItem, quantity, comments);
            HttpContext.Session.SetTakeOrderViewModel(vm);
            TempData["Success"] = $"Added {quantity}x {menuItem.Description} to order.";
            return RedirectToAction(nameof(TakeOrder), new { tableNumber });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter")]
        public IActionResult IncreaseQuantity(int tableNumber, int menuItemID)
        {
            var vm = HttpContext.Session.GetTakeOrderViewModel(tableNumber);
            var item = vm.CurrentOrderItems.FirstOrDefault(i => i.MenuItemID == menuItemID);
            if (item != null) item.Quantity++;
            HttpContext.Session.SetTakeOrderViewModel(vm);
            return RedirectToAction(nameof(TakeOrder), new { tableNumber });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter")]
        public IActionResult DecreaseQuantity(int tableNumber, int menuItemID)
        {
            var vm = HttpContext.Session.GetTakeOrderViewModel(tableNumber);
            var item = vm.CurrentOrderItems.FirstOrDefault(i => i.MenuItemID == menuItemID);
            if (item != null)
            {
                if (item.Quantity <= 1) vm.CurrentOrderItems.Remove(item);
                else item.Quantity--;
                HttpContext.Session.SetTakeOrderViewModel(vm);
            }
            return RedirectToAction(nameof(TakeOrder), new { tableNumber });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter")]
        public IActionResult RemoveFromOrder(int tableNumber, int menuItemID)
        {
            var vm = HttpContext.Session.GetTakeOrderViewModel(tableNumber);
            vm.RemoveItem(menuItemID, removeAll: true);
            HttpContext.Session.SetTakeOrderViewModel(vm);
            TempData["Success"] = "Item removed from order.";
            return RedirectToAction(nameof(TakeOrder), new { tableNumber });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter")]
        public IActionResult UpdateComment(int tableNumber, int menuItemID, string comment)
        {
            var vm = HttpContext.Session.GetTakeOrderViewModel(tableNumber);
            var item = vm.CurrentOrderItems.FirstOrDefault(i => i.MenuItemID == menuItemID);
            if (item != null)
            {
                item.Comments = comment;
                HttpContext.Session.SetTakeOrderViewModel(vm);
            }
            return RedirectToAction(nameof(TakeOrder), new { tableNumber });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter")]
        public IActionResult SaveOrder(int tableNumber)
        {
            var vm = HttpContext.Session.GetTakeOrderViewModel(tableNumber);
            if (vm.CurrentOrderItems == null || !vm.CurrentOrderItems.Any())
            {
                TempData["Error"] = "Cannot save an empty order!";
                return RedirectToAction(nameof(TakeOrder), new { tableNumber });
            }
            try
            {
                _orderService.SaveOrder(tableNumber, vm.CurrentOrderItems);
                _menuService.DecreaseStockForOrder(vm.CurrentOrderItems);
                HttpContext.Session.ClearCurrentOrder();
                TempData["Success"] = $"Order for Table {tableNumber} saved successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to save order: {ex.Message}";
                return RedirectToAction(nameof(TakeOrder), new { tableNumber });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter")]
        public IActionResult CancelOrder(int tableNumber)
        {
            HttpContext.Session.ClearCurrentOrder();
            TempData["Success"] = "Current order has been cancelled.";
            return RedirectToAction(nameof(Index));
        }
    }
}