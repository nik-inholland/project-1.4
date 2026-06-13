using Microsoft.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.repo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("ChapeauConnection");
        }
        public OrderTable? GetById(int id)
        {
            OrderTable? order = null;

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"SELECT TableOrderID,
                         TotalPrice,
                         PaymentID,
                         OrderStatus,
                         CreatedAt
                  FROM TableOrder
                  WHERE TableOrderID = @id";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                order = ReadOrder(reader);
            }

            return order;
        }

        public List<OrderTable> GetAllTableOrders()
        {
            var orders = new List<OrderTable>();
            using var connection = new SqlConnection(_connectionString);
            string query = @"SELECT TableOrderID, TotalPrice, PaymentID, OrderStatus, CreatedAt 
                     FROM TableOrder ORDER BY CreatedAt DESC";
            using var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
                orders.Add(ReadOrder(reader));
            return orders;
        }
        public List<OrderTable> GetRecentTableOrders(int count = 10)
        {
            var orders = new List<OrderTable>();
            using var connection = new SqlConnection(_connectionString);
            string query = @$"SELECT TOP {count} TableOrderID, TotalPrice, PaymentID, OrderStatus, CreatedAt 
                      FROM TableOrder ORDER BY CreatedAt DESC";
            using var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
                orders.Add(ReadOrder(reader));
            return orders;
        }

        public List<PersonOrder> GetPersonOrdersByTableId(int tableOrderId)
        {
            List<PersonOrder> persons = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"SELECT PersonOrderID,
                         TableOrderID,
                         PersonName,
                         TotalPrice,
                         PaymentID,
                         OrderStatus,
                         CreatedAt
                  FROM PersonOrder
                  WHERE TableOrderID = @id";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", tableOrderId);

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                persons.Add(ReadPersonOrder(reader));
            }

            return persons;
        }

        public void UpdateOrderStatus(OrderTable order)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"UPDATE TableOrder 
                     SET OrderStatus = @status
                     WHERE TableOrderID = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", (int)order.OrderStatus);
            cmd.Parameters.AddWithValue("@id", order.TableOrderID);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdatePersonOrderStatus(PersonOrder po)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"UPDATE PersonOrder 
                     SET OrderStatus = @status
                     WHERE PersonOrderID = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", (int)po.OrderStatus);
            cmd.Parameters.AddWithValue("@id", po.PersonOrderID);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        private int GetInt(SqlDataReader r, string col)
        {
            return r[col] == DBNull.Value ? 0 : (int)r[col];
        }

        private decimal GetDecimal(SqlDataReader r, string col)
        {
            return r[col] == DBNull.Value ? 0m : (decimal)r[col];
        }

        private DateTime GetDateTime(SqlDataReader r, string col)
        {
            return r[col] == DBNull.Value ? DateTime.MinValue : (DateTime)r[col];
        }

        private string GetString(SqlDataReader r, string col)
        {
            return r[col] == DBNull.Value ? "" : r[col].ToString();
        }

        private OrderTable ReadOrder(SqlDataReader reader)
        {
            return new OrderTable
            {
                TableOrderID = GetInt(reader, "TableOrderID"),
                TotalPrice = GetDecimal(reader, "TotalPrice"),
                PaymentID = GetInt(reader, "PaymentID"),
                OrderStatus = (OrderStatus)GetInt(reader, "OrderStatus"),
                CreatedAt = GetDateTime(reader, "CreatedAt")
            };
        }

        private PersonOrder ReadPersonOrder(SqlDataReader reader)
        {
            return new PersonOrder
            {
                PersonOrderID = (int)reader["PersonOrderID"],
                TableOrderID = (int)reader["TableOrderID"],
                PersonName = reader["PersonName"].ToString(),

                TotalPrice = reader["TotalPrice"] == DBNull.Value ? 0m : (decimal)reader["TotalPrice"],

                PaymentID = reader["PaymentID"] == DBNull.Value ? 0 : (int)reader["PaymentID"],

                OrderStatus = (OrderStatus)(int)reader["OrderStatus"],

                CreatedAt = reader["CreatedAt"] == DBNull.Value
                    ? DateTime.MinValue
                    : (DateTime)reader["CreatedAt"]
            };
        }

        public List<OrderItem> GetOrderItemsByPersonOrderId(int personOrderId)
        {
            var items = new List<OrderItem>();

            string query = @"
        SELECT OrderItemID, itemName, Comments, PricePerItem, vat_category,
               Category, Quantity, MenuItemId, TimePlaced, PersonOrderId
        FROM OrderItems
        WHERE PersonOrderId = @personOrderId
        ORDER BY OrderItemID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@personOrderId", personOrderId);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
                items.Add(ReadOrderItem(reader));

            return items;
        }

        // Helper (add to repository)
        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            return new OrderItem
            {
                OrderItemID = GetInt(reader, "OrderItemID"),
                Name = GetString(reader, "itemName"),
                Comments = GetString(reader, "Comments"),
                Price = (double)GetDecimal(reader, "PricePerItem"),
                VatCategory = reader["vat_category"] != DBNull.Value && (bool)reader["vat_category"],
                Category = GetInt(reader, "Category"),
                Quantity = GetInt(reader, "Quantity"),
                MenuItemId = GetInt(reader, "MenuItemId"),
                PlacedAt = GetDateTime(reader, "TimePlaced"),
                PersonOrderId = GetInt(reader, "PersonOrderId")
            };
        }
    }
}