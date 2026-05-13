using Microsoft.Data.SqlClient;
using WebApplication3.Models;
using WebApplication3.repo;

namespace WebApplication3.Repo.Folder_OrderItem
{
    public class DBOrderItemRepo : Iorder_item_managment
    {
        private readonly string _connectionString;

        public DBOrderItemRepo(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection");
        }

        public List<OrderItem> GetAll()
        {
            List<OrderItem> items = new List<OrderItem>();

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT item_id, dish_name, detail,
                             cost, vat, stock, [type]
                      FROM order_item
                      ORDER BY dish_name";

                SqlCommand command =
                    new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(ReadOrderItem(reader));
                }
            }

            return items;
        }

        public OrderItem? GetById(int id)
        {
            OrderItem? item = null;

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT item_id, dish_name, detail,
                             cost, vat, stock, [type]
                      FROM order_item
                      WHERE item_id = @id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", id);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();

                if (reader.Read())
                {
                    item = ReadOrderItem(reader);
                }
            }

            return item;
        }

        public void Add(OrderItem orderItem)
        {
            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"INSERT INTO order_item
                      (dish_name, detail, cost, vat, stock, [type])

                      VALUES
                      (@dish_name, @detail, @cost,
                       @vat, @stock, @type)";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@dish_name",
                    orderItem.Dish_name);

                command.Parameters.AddWithValue("@detail",
                    orderItem.Details);

                command.Parameters.AddWithValue("@cost",
                    orderItem.Price);

                command.Parameters.AddWithValue("@vat",
                    orderItem.VAT);

                command.Parameters.AddWithValue("@stock",
                    orderItem.Stock);

                command.Parameters.AddWithValue("@type",
                    orderItem.Type);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void Update(OrderItem orderItem)
        {
            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"UPDATE order_item
                      SET dish_name = @dish_name,
                          detail = @detail,
                          cost = @cost,
                          vat = @vat,
                          stock = @stock,
                          [type] = @type
                      WHERE item_id = @id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@id",
                    orderItem.Item_id);

                command.Parameters.AddWithValue("@dish_name",
                    orderItem.Dish_name);

                command.Parameters.AddWithValue("@detail",
                    orderItem.Details);

                command.Parameters.AddWithValue("@cost",
                    orderItem.Price);

                command.Parameters.AddWithValue("@vat",
                    orderItem.VAT);

                command.Parameters.AddWithValue("@stock",
                    orderItem.Stock);

                command.Parameters.AddWithValue("@type",
                    orderItem.Type);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    "DELETE FROM order_item WHERE item_id = @id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", id);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        private OrderItem ReadOrderItem(SqlDataReader reader)
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

        public void Create(OrderItem orderItem)
        {
            throw new NotImplementedException();
        }

        public void Delete(OrderItem orderItem)
        {
            throw new NotImplementedException();
        }
    }
}