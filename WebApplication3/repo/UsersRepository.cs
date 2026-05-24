using Microsoft.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.repo
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection");
        }

        public List<Employee> GetAll()
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT employeeID,
                             employeeType,
                             firstName,
                             lastName,
                             dateOfBirth,
                             password,
                             Username
                      FROM Employee";

                SqlCommand command =
                    new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(ReadEmployee(reader));
                }
            }

            return employees;
        }

        public Employee? GetById(int userId)
        {
            Employee? employee = null;

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT employeeID,
                             employeeType,
                             firstName,
                             lastName,
                             dateOfBirth,
                             password,
                             Username
                      FROM Employee
                      WHERE employeeID = @id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", userId);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();

                if (reader.Read())
                {
                    employee = ReadEmployee(reader);
                }
            }

            return employee;
        }

        public Employee? GetByLoginCredentials(
            string userName,
            string password)
        {
            Employee? employee = null;

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT employeeID,
                             employeeType,
                             firstName,
                             lastName,
                             dateOfBirth,
                             password,
                             Username
                      FROM Employee
                      WHERE Username = @username
                      AND password = @password";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@username", userName);

                command.Parameters.AddWithValue("@password", password);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();

                if (reader.Read())
                {
                    employee = ReadEmployee(reader);
                }
            }

            return employee;
        }

        public void Create(Employee user)
        {
            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"INSERT INTO Employee
                      (employeeType,
                       firstName,
                       lastName,
                       dateOfBirth,
                       password,
                       Username)

                      VALUES
                      (@employeeType,
                       @firstName,
                       @lastName,
                       @dateOfBirth,
                       @password,
                       @username)";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@employeeType",
                    user.EmployeeType);

                command.Parameters.AddWithValue("@firstName",
                    user.FirstName);

                command.Parameters.AddWithValue("@lastName",
                    user.LastName);

                command.Parameters.AddWithValue("@dateOfBirth",
                    user.DateOfBirth);

                command.Parameters.AddWithValue("@password",
                    user.Password);

                command.Parameters.AddWithValue("@username",
                    user.Username);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void Update(Employee user)
        {
            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"UPDATE Employee
                      SET employeeType = @employeeType,
                          firstName = @firstName,
                          lastName = @lastName,
                          dateOfBirth = @dateOfBirth,
                          password = @password,
                          Username = @username
                      WHERE employeeID = @id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@id",
                    user.EmployeeID);

                command.Parameters.AddWithValue("@employeeType",
                    user.EmployeeType);

                command.Parameters.AddWithValue("@firstName",
                    user.FirstName);

                command.Parameters.AddWithValue("@lastName",
                    user.LastName);

                command.Parameters.AddWithValue("@dateOfBirth",
                    user.DateOfBirth);

                command.Parameters.AddWithValue("@password",
                    user.Password);

                command.Parameters.AddWithValue("@username",
                    user.Username);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void Delete(Employee user)
        {
            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"DELETE FROM Employee
                      WHERE employeeID = @id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@id",
                    user.EmployeeID);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public bool UsernameExists(string username)
        {
            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT COUNT(*)
                      FROM Employee
                      WHERE Username = @username";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@username",
                    username);

                connection.Open();

                int count =
                    (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        private Employee ReadEmployee(SqlDataReader reader)
        {
            return new Employee(
                (int)reader["employeeID"],
                (string)reader["employeeType"],
                (string)reader["firstName"],
                (string)reader["lastName"],
                (DateTime)reader["dateOfBirth"],
                (string)reader["password"],
                (string)reader["Username"]
            );
        }
    }
}