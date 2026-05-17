using Microsoft.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.repo
{
    public class order_item_managment : Iorder_item_managment
    {
        private readonly string _connectionString;

        public order_item_managment(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public void Create(OrderItem orderItem)
        {
            throw new NotImplementedException();
        }

        public List<OrderItem> GetAll()
        {
            List<OrderItem> activities = new List<OrderItem>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT item_id, dish_name, detail, cost, vat, stock, [type] FROM MenuItem ORDER BY dish_name;";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    activities.Add(ReadOrderItem(reader));
                }
            }
            return activities;
        }

        public OrderItem? GetById(int userid)
        {
            throw new NotImplementedException();
        }

        public void Update(OrderItem orderItem)
        {
            throw new NotImplementedException();
        }

        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            return new OrderItem(
                (int)reader["menuItemID"],
                (string)reader["description"],
                (double)reader["price"],
                (bool)reader["vat_category"],
                (int)reader["course_type"],
                (int)reader["quantity"]
            );
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
