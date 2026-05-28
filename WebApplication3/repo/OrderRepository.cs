using Microsoft.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.repo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public OrderTable? GetById(int id)
        {
            OrderTable? order = null;
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"SELECT TableOrderID, TotalPrice, PaymentID, OrderStatus, OrderDateTime
                             FROM OrderTable WHERE TableOrderID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                order = ReadOrder(reader);
            }
            return order;
        }

        public List<OrderTable> GetAll()
        {
            List<OrderTable> orders = new();
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"SELECT TableOrderID, TotalPrice, PaymentID, OrderStatus, OrderDateTime
                             FROM OrderTable ORDER BY OrderDateTime DESC";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(ReadOrder(reader));
            }
            return orders;
        }

        public List<PersonOrder> GetPersonOrdersByTableId(int tableOrderId)
        {
            List<PersonOrder> persons = new();
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"SELECT PersonOrderID, TableOrderID, PersonName, TotalPrice, PaymentID, OrderStatus, CreatedAt
                             FROM PersonOrder WHERE TableOrderID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", tableOrderId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                persons.Add(ReadPersonOrder(reader));
            }
            return persons;
        }

        public void UpdateOrderStatus(int orderId, int status)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"UPDATE OrderTable SET OrderStatus = @status WHERE TableOrderID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", orderId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void UpdatePersonOrderStatus(int personOrderId, int status)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"UPDATE PersonOrder SET OrderStatus = @status WHERE PersonOrderID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", personOrderId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        // === ADDED FOR STUDENT 2 - TAKING ORDER ===
        public int CreateOrder(int tableId, int employeeId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"INSERT INTO OrderTable (TableID, EmployeeID, OrderDateTime, OrderStatus) 
                             OUTPUT INSERTED.TableOrderID 
                             VALUES (@tableId, @employeeId, GETDATE(), 0)";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@tableId", tableId);
            command.Parameters.AddWithValue("@employeeId", employeeId);

            connection.Open();
            return (int)command.ExecuteScalar();
        }

        public void AddOrderItem(int orderId, OrderItem item)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"INSERT INTO OrderItem (OrderID, MenuItemID, Quantity, Comment) 
                             VALUES (@orderId, @menuItemId, @quantity, @comment)";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@orderId", orderId);
            command.Parameters.AddWithValue("@menuItemId", item.MenuItemID);
            command.Parameters.AddWithValue("@quantity", item.Quantity);
            command.Parameters.AddWithValue("@comment", (object?)item.Comment ?? DBNull.Value);

            connection.Open();
            command.ExecuteNonQuery();
        }

        // Helper methods (already existed)
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
                OrderDateTime = GetDateTime(reader, "OrderDateTime")
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
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["CreatedAt"]
            };
        }
    }
}