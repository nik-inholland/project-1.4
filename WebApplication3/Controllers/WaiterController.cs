using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    public class WaiterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
