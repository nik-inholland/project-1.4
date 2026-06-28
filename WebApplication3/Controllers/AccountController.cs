using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IEmployeeService employeeService, ILogger<AccountController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var employee = _employeeService.Authenticate(model.Username, model.Password);

                if (employee == null)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    _logger.LogWarning("Failed login attempt for user {Username}", model.Username);
                    return View(model);
                }

                await _employeeService.SignInUser(employee, HttpContext);

                HttpContext.Session.SetString("Username", employee.Username);
                HttpContext.Session.SetInt32("EmployeeID", employee.EmployeeID);
                HttpContext.Session.SetString("Role", employee.EmployeeType ?? "waiter");

                _logger.LogInformation("User {Username} logged in successfully", employee.Username);

                return RedirectToAction("Index", "Waiter");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user {Username}", model.Username);
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.Session.Clear();

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogInformation("User logged out successfully");

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                return RedirectToAction("Index", "Home");
            }
        }
    }
}