using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebApplication3.Helpers;
using WebApplication3.Models;
using WebApplication3.repo;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmployeeService _repository;

        public AccountController(IEmployeeService repository)
        {
            _repository = repository;
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
            {
                return View(model);
            }

            string hashedPassword =
                PasswordHelper.HashPassword(model.Password);

            Employee? employee =
                _repository.GetByLoginCredentials(
                    model.Username,
                    hashedPassword);

            if (employee == null)
            {
                ViewBag.Error =
                    "Invalid username or password";

                return View(model);
            }

            HttpContext.Session.SetString(
                "Username",
                employee.Username);

            HttpContext.Session.SetInt32(
                "EmployeeID",
                employee.EmployeeID);

            HttpContext.Session.SetString(
                "EmployeeType",
                employee.EmployeeType.Trim().ToLower()
            );

            return RedirectToAction(
                "Index",
                "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}