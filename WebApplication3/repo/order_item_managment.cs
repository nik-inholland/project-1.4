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

        public void Delete(OrderItem orderItem)
        {
            throw new NotImplementedException();
        }

        public List<OrderItem> GetAll()
        {
            List<OrderItem> activities = new List<OrderItem>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT item_id, dish_name, detail, cost, vat, stock, [type] FROM order_item ORDER BY dish_name;";
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

        public OrderItem ReadOrderItem(SqlDataReader reader)
        {
            return new OrderItem(
                (int)reader["item_id"],
                (string)reader["dish_name"],
                (string)reader["detail"],
                (decimal)reader["cost"],
                (decimal)reader["vat"],
                (int)reader["stock"],
                (string)reader["type"]
            );
        }
    }
}
