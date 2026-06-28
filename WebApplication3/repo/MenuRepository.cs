using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication3.Models;
using WebApplication3.repo.@interface;

namespace WebApplication3.repo
{
    public class MenuRepository : IMenuRepository
    {
        private readonly string _connectionString;

        public MenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauConnection");
        }

        public List<MenuItem> GetAllMenuItems()
        {
            var menuItems = new List<MenuItem>();
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT * FROM MenuItem";
            using var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
                menuItems.Add(ReadMenuItem(reader));
            return menuItems;
        }

        public List<MenuItem> GetMenuItemsByCardType(string cardType)
        {
            return GetAllMenuItems()
                .Where(m => m.CardType == cardType)
                .ToList();
        }

        public List<MenuItem> GetMenuItemsByCategory(int category)
        {
            var menuItems = new List<MenuItem>();
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT * FROM MenuItem WHERE course_type = @category";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@category", category);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
                menuItems.Add(ReadMenuItem(reader));
            return menuItems;
        }

        public MenuItem? GetMenuItemById(int menuItemID)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT * FROM MenuItem WHERE menuItemID = @id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", menuItemID);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
                return ReadMenuItem(reader);
            return null;
        }

        public void UpdateStock(int menuItemID, int quantityChange)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "UPDATE MenuItem SET quantity = quantity + @change WHERE menuItemID = @id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@change", quantityChange);
            command.Parameters.AddWithValue("@id", menuItemID);
            connection.Open();
            command.ExecuteNonQuery();
        }

        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            int courseType = Convert.ToInt32(reader["course_type"]);
            return new MenuItem
            {
                MenuItemID = Convert.ToInt32(reader["menuItemID"]),
                Description = reader["description"].ToString() ?? "",
                Price = Convert.ToDecimal(reader["price"]),
                VatCategory = Convert.ToBoolean(reader["vat_category"]),
                CourseType = courseType,
                QuantityInStock = Convert.ToInt32(reader["quantity"]),
                CardType = GetCardTypeFromCourse(courseType)
            };
        }

        private string GetCardTypeFromCourse(int courseType)
        {
            if (courseType >= 8 && courseType <= 13) return "Drinks";
            if (courseType >= 4 && courseType <= 7) return "Diner";
            if (courseType >= 1 && courseType <= 3) return "Lunch";
            return "All";
        }
    }
}