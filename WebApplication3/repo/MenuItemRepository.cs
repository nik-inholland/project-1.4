using Microsoft.Data.SqlClient;
using WebApplication3.Models;
using WebApplication3.repo.@interface;

namespace WebApplication3.repo
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly string _connectionString;

        public MenuItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauConnection");
        }

        public IEnumerable<MenuItem> GetAll()
        {
            var items = new List<MenuItem>();
            using var connection = new SqlConnection(_connectionString);
            string query = @"
                SELECT menuItemID, description, price, vat_category, course_type, quantity
                FROM MenuItem
                ORDER BY description";

            using var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(ReadMenuItem(reader));
            }
            return items;
        }
        public MenuItem? GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"
                SELECT menuItemID, description, price, vat_category, course_type, quantity
                FROM MenuItem
                WHERE menuItemID = @id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ReadMenuItem(reader);
            }
            return null;
        }

        public void Add(MenuItem item)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"
                INSERT INTO MenuItem (description, price, vat_category, course_type, quantity)
                VALUES (@description, @price, @vat_category, @course_type, @quantity);
                SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@price", item.Price);
            command.Parameters.AddWithValue("@vat_category", item.VatCategory);
            command.Parameters.AddWithValue("@course_type", item.CourseType);
            command.Parameters.AddWithValue("@quantity", item.QuantityInStock);

            connection.Open();
            var newId = command.ExecuteScalar();
            if (newId != null)
            {
                item.MenuItemID = Convert.ToInt32(newId);
            }
        }

        public void Update(MenuItem item)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"
                UPDATE MenuItem
                SET description = @description,
                    price = @price,
                    vat_category = @vat_category,
                    course_type = @course_type,
                    quantity = @quantity
                WHERE menuItemID = @id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", item.MenuItemID);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@price", item.Price);
            command.Parameters.AddWithValue("@vat_category", item.VatCategory);
            command.Parameters.AddWithValue("@course_type", item.CourseType);
            command.Parameters.AddWithValue("@quantity", item.QuantityInStock);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "DELETE FROM MenuItem WHERE menuItemID = @id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public IEnumerable<MenuItem> GetByCourseType(int courseType)
        {
            var items = new List<MenuItem>();
            using var connection = new SqlConnection(_connectionString);
            string query = @"
                SELECT menuItemID, description, price, vat_category, course_type, quantity
                FROM MenuItem
                WHERE course_type = @courseType
                ORDER BY description";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@courseType", courseType);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(ReadMenuItem(reader));
            }
            return items;
        }

        public IEnumerable<MenuItem> GetInStock()
        {
            var items = new List<MenuItem>();
            using var connection = new SqlConnection(_connectionString);
            string query = @"
                SELECT menuItemID, description, price, vat_category, course_type, quantity
                FROM MenuItem
                WHERE quantity > 0
                ORDER BY description";

            using var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(ReadMenuItem(reader));
            }
            return items;
        }

        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            return new MenuItem
            {
                MenuItemID = Convert.ToInt32(reader["menuItemID"]),
                Description = Convert.ToString(reader["description"]) ?? string.Empty,
                Price = Convert.ToDecimal(reader["price"]),
                VatCategory = Convert.ToBoolean(reader["vat_category"]),
                CourseType = Convert.ToInt32(reader["course_type"]),
                QuantityInStock = Convert.ToInt32(reader["quantity"])
            };
        }
    }
}