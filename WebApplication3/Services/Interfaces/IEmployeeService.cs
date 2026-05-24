using WebApplication3.Models;

namespace WebApplication3.Services.Interfaces
{
    public interface IEmployeeService
    {
        List<Employee> GetAll();
        Employee? GetById(int userId);
        Employee? GetByLoginCredentials(string userName, string password);
        void Create(Employee user);
        void Update(Employee user);
        void Delete(Employee user);
        bool UsernameExists(string Username);

    }
}
