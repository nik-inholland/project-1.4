using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.Services.Interfaces;
using WebApplication3.Exceptions;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPersonOrderService _personOrderService;

        public OrderController(IOrderService orderService, IPersonOrderService personOrderService)
        {
            _orderService = orderService;
            _personOrderService = personOrderService;
        }

        [Authorize]
        public IActionResult Index(bool showClosed = false, string? dateFilter = null)
        {
            try
            {
                var orders = _orderService.GetRecentTableOrders(10, showClosed);

                if (!string.IsNullOrEmpty(dateFilter) && DateTime.TryParse(dateFilter, out DateTime filterDate))
                {
                    orders = orders.Where(o => o.CreatedAt.Date == filterDate.Date).ToList();
                }

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
                TempData["Error"] = "Unable to update order status.";
            }

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
            try
            {
                _orderService.CloseOrder(id);
                TempData["Success"] = "Order closed.";
            }
            catch (NotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            catch { TempData["Error"] = "Unable to close order."; }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "admin, waiter, chef, barstaff")]
        public IActionResult ReopenOrder(int id)
        {
            try
            {
                _orderService.ReopenOrder(id);
                TempData["Success"] = "Order reopened.";
            }
            catch (NotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
            catch { TempData["Error"] = "Unable to reopen order."; }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}