using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Exceptions;
using WebApplication3.Helpers;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.Services.@interface;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;

        public OrderController(
            IOrderService orderService,
            IMenuService menuService)
        {
            _orderService = orderService;
            _menuService = menuService;
        }

        [Authorize]
        public IActionResult Index(bool showClosed = false, string? dateFilter = null, int count = 10)
        {
            try
            {
                var orders = _orderService.GetRecentTableOrders(count, showClosed, dateFilter);

                var viewModels = orders.Select(o => new OrderListViewModel
                {
                    TableOrderID = o.TableOrderID,
                    TableNumber = o.TableNumber,
                    Status = o.orderStatus,
                    CreatedAt = o.CreatedAt,
                    IsClosed = o.IsClosed,
                }).ToList();

                ViewBag.ShowClosed = showClosed;
                ViewBag.DateFilter = dateFilter;
                ViewBag.Count = count;

                return View(viewModels);
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
                var viewModel = _orderService.GetOrderDetails(id);
                if (viewModel == null) return NotFound();
                return View(viewModel);
            }
            catch
            {
                TempData["Error"] = "Unable to load order details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter, chef, barstaff")]
        public IActionResult ToggleOrderStatus(int tableOrderID)
        {
            try
            {
                _orderService.ToggleOrderStatus(tableOrderID);
                TempData["Success"] = "Order status toggled.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch
            {
                TempData["Error"] = "Unable to toggle order status.";
            }
            return RedirectToAction(nameof(Details), new { id = tableOrderID });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter, chef, barstaff")]
        public IActionResult ToggleItemStatus(int orderId, int orderItemId)
        {
            try
            {
                _orderService.ToggleItemStatus(orderItemId);
                TempData["Success"] = "Item status toggled.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch
            {
                TempData["Error"] = "Unable to toggle item status.";
            }
            return RedirectToAction(nameof(Details), new { id = orderId });
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
                var order = new OrderTable
                {
                    TableNumber = tableNumber,
                    OrderItems = vm.CurrentOrderItems,
                    CreatedAt = DateTime.Now,
                    PaymentID = 0,
                    orderStatus = OrderStatus.Ordered
                };

                _orderService.SaveOrder(order);

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

        [AllowAnonymous]
        public IActionResult TestOrderItems(int id)
        {
            var items = _orderService.GetOrderItems(id);
            return View(items);
        }
    }
}