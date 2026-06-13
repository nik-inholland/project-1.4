using WebApplication3.Helpers;
using WebApplication3.Models;
using WebApplication3.repo;
using WebApplication3.Services.Interfaces;

namespace WebApplication3.Services


{
    public class EmployeeService : IEmployeeService
    {
        private IUsersRepository _usersRepository;

        public EmployeeService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
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

        public Employee? GetByLoginCredentials(string userName, string plainPassword)
        {
            string hashed = PasswordHelper.HashPassword(plainPassword);

            return _usersRepository.GetByLoginCredentials(userName, hashed);
        }

        public bool UsernameExists(string Username)
        {
            return _usersRepository.UsernameExists(Username);
        }
    }
}
