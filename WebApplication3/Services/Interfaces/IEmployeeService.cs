using Microsoft.AspNetCore.Http;
using WebApplication3.Models;

namespace WebApplication3.Services.Interfaces
{
    public interface IEmployeeService
    {
        Employee? Authenticate(string username, string password);
        Task SignInUser(Employee employee, HttpContext httpContext);
        List<Employee> GetAll();
        Employee? GetById(int userId);
        void Create(Employee user);
        void Update(Employee user);
        void Delete(Employee user);
        bool UsernameExists(string Username);
    }
}