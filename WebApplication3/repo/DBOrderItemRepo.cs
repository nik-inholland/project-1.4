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
                    @"SELECT menuItemID,
                             description,
                             price,
                             vat_category,
                             course_type,
                             quantity
                      FROM MenuItem
                      ORDER BY menuItemID";

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
                    @"SELECT menuItemID,
                             description,
                             price,
                             vat_category,
                             course_type,
                             quantity
                      FROM MenuItem
                      WHERE menuItemID = @id";

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

        public void Create(OrderItem orderItem)
        {
            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"INSERT INTO MenuItem
                      (description,
                       price,
                       vat_category,
                       course_type,
                       quantity)

                      VALUES
                      (@description,
                       @price,
                       @vat_category,
                       @course_type,
                       @quantity)";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@description",
                    orderItem.Description);

                command.Parameters.AddWithValue("@price",
                    orderItem.Price);

                command.Parameters.AddWithValue("@vat_category",
                    orderItem.VatCategory);

                command.Parameters.AddWithValue("@course_type",
                    orderItem.CourseType);

                command.Parameters.AddWithValue("@quantity",
                    orderItem.Quantity);

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
                    @"UPDATE MenuItem
                      SET description = @description,
                          price = @price,
                          vat_category = @vat_category,
                          course_type = @course_type,
                          quantity = @quantity
                      WHERE menuItemID = @id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@id",
                    orderItem.MenuItemID);

                command.Parameters.AddWithValue("@description",
                    orderItem.Description);

                command.Parameters.AddWithValue("@price",
                    orderItem.Price);

                command.Parameters.AddWithValue("@vat_category",
                    orderItem.VatCategory);

                command.Parameters.AddWithValue("@course_type",
                    orderItem.CourseType);

                command.Parameters.AddWithValue("@quantity",
                    orderItem.Quantity);

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
                    @"DELETE FROM MenuItem
                      WHERE menuItemID = @id";

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
                (int)reader["menuItemID"],
                (string)reader["description"],
                (double)reader["price"],
                (bool)reader["vat_category"],
                (int)reader["course_type"],
                (int)reader["quantity"]
            );
        }
    }
}