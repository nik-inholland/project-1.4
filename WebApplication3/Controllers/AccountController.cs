using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public AccountController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Employee? employee = _employeeService.GetByLoginCredentials(
                model.Username, model.Password);

            if (employee is null)
            {
                ViewBag.Error = "Invalid username or password";
                return View(model);
            }

            SignInUser(employee);

            HttpContext.Session.SetString("Username", employee.Username);
            HttpContext.Session.SetInt32("EmployeeID", employee.EmployeeID);

            return RedirectToAction("Index", "Home");
        }

        private async void SignInUser(Employee employee)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.EmployeeID.ToString()),
                new Claim(ClaimTypes.Name, employee.Username),
                new Claim(ClaimTypes.Role, employee.EmployeeType.Trim().ToLower()) 
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}