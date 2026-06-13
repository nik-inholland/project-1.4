using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.repo;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _repository;

        public EmployeesController(IEmployeeService repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "admin, manager")]
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            return View();
        }

        [Authorize(Roles = "admin, manager")]
        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            bool usernameExists =
                _repository.UsernameExists(employee.Username);

            if (usernameExists)
            {
                ViewBag.Error =
                    "Username already exists";

                return View(employee);
            }

            _repository.Create(employee);

            return RedirectToAction(
                "Index",
                "Home");
        }
    }
}