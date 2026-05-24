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
        public List<Employee> GetAll()
        {
            return _usersRepository.GetAll();
        }
        public Employee? GetById(int userId)
        {
            return _usersRepository.GetById(userId);
        }
        public void Create(Employee user)
        {
            _usersRepository.Create(user);
        }
        public void Update(Employee user)
        {
            _usersRepository.Update(user);
        }
        public void Delete(Employee user)
        {
            _usersRepository.Delete(user);
        }

        public Employee? GetByLoginCredentials(string userName, string password)
        {
            return _usersRepository.GetByLoginCredentials(userName, password);
        }

        public bool UsernameExists(string Username)
        {
            return _usersRepository.UsernameExists(Username);
        }
    }
}
