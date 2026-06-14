using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WebApplication3.Helpers;
using WebApplication3.Models;
using WebApplication3.repo.@interface;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IUsersRepository usersRepository, ILogger<EmployeeService> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public Employee? Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            string hashedPassword = PasswordHelper.HashPassword(password);
            return _usersRepository.GetByLoginCredentials(username, hashedPassword);
        }

        public async Task SignInUser(Employee employee, HttpContext httpContext)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.EmployeeID.ToString()),
                new Claim(ClaimTypes.Name, employee.Username),
                new Claim(ClaimTypes.Role, employee.EmployeeType?.Trim().ToLower() ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);
        }

        public List<Employee> GetAll() => _usersRepository.GetAll();

        public Employee? GetById(int userId) => _usersRepository.GetById(userId);

        public void Update(Employee user) => _usersRepository.Update(user);

        public void Delete(Employee user) => _usersRepository.Delete(user);

        public void Create(Employee user)
        {
            user.Password = PasswordHelper.HashPassword(user.Password);
            _usersRepository.Create(user);
        }

        public bool UsernameExists(string Username)
        {
            return _usersRepository.UsernameExists(Username);
        }
    }
}