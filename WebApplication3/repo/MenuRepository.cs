using Microsoft.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.repo
{
    public class MenuRepository : IMenuRepository
    {
        private readonly string _connectionString;

        public MenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<MenuItem> GetAllActive()
        {
            List<MenuItem> items = new();
            using SqlConnection conn = new(_connectionString);
            string query = @"SELECT * FROM MenuItem WHERE IsActive = 1 ORDER BY Name";
            SqlCommand cmd = new(query, conn);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(ReadMenuItem(reader));
            }
            return items;
        }

        public List<MenuItem> GetFiltered(string cardType, string category)
        {
            List<MenuItem> items = new();
            using SqlConnection conn = new(_connectionString);
            string query = @"SELECT * FROM MenuItem 
                             WHERE IsActive = 1";

            if (cardType != "All")
                query += " AND CardType = @cardType";
            if (category != "All")
                query += " AND Category = @category";

            query += " ORDER BY Name";

            SqlCommand cmd = new(query, conn);

            if (cardType != "All")
                cmd.Parameters.AddWithValue("@cardType", cardType);
            if (category != "All")
                cmd.Parameters.AddWithValue("@category", category);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(ReadMenuItem(reader));
            }
            return items;
        }

        public MenuItem? GetById(int id)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "SELECT * FROM MenuItem WHERE MenuItemID = @id";
            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
                return ReadMenuItem(reader);
            return null;
        }

        public void DecreaseStock(int menuItemId, int quantity)
        {
            using SqlConnection conn = new(_connectionString);
            string query = "UPDATE MenuItem SET Stock = Stock - @qty WHERE MenuItemID = @id";
            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@qty", quantity);
            cmd.Parameters.AddWithValue("@id", menuItemId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            return new MenuItem
            {
                MenuItemID = (int)reader["MenuItemID"],
                Name = reader["Name"].ToString() ?? "",
                Price = (decimal)reader["Price"],
                CardType = reader["CardType"].ToString() ?? "",
                Category = reader["Category"].ToString() ?? "",
                Stock = (int)reader["Stock"],
                IsActive = (bool)reader["IsActive"]
            };
        }
    }
}