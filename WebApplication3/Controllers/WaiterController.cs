using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.Services.Interfaces;
using WebApplication3.Services.@interface;

namespace WebApplication3.Controllers
{
    [Authorize(Roles = "admin, waiter")]
    public class WaiterController : Controller
    {
        private readonly ITableService _tableService;
        private readonly IOrderService _orderService;

        public WaiterController(ITableService tableService, IOrderService orderService)
        {
            _tableService = tableService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}