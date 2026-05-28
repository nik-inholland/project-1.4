namespace WebApplication3.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        public string EmployeeType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }

        public Employee()
        {
        }

        public Employee(
            int employeeID,
            string employeeType,
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            string password,
            string username)
        {
            EmployeeID = employeeID;
            EmployeeType = employeeType;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Password = password;
            Username = username;
        }
    }
}