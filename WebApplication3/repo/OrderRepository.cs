using Microsoft.Data.SqlClient;
using WebApplication3.Models;
using WebApplication3.repo.@interface;

namespace WebApplication3.repo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauConnection");
        }
        public OrderTable? GetById(int id)
        {
            OrderTable? order = null;
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"SELECT TableOrderID, TableNumber, TotalPrice, PaymentID, OrderStatus, CreatedAt, ClosedAt FROM TableOrder WHERE TableOrderID = @id";

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

        public List<OrderTable> GetAllTableOrders()
        {
            var orders = new List<OrderTable>();
            using var connection = new SqlConnection(_connectionString);
            string query = @"SELECT TableOrderID, TableNumber, TotalPrice, PaymentID, OrderStatus, CreatedAt, ClosedAt FROM TableOrder ORDER BY CreatedAt DESC, TableNumber ASC";
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
            string query = @$"SELECT TOP {count} TableOrderID, TableNumber, TotalPrice, PaymentID, OrderStatus, CreatedAt, ClosedAt FROM TableOrder ORDER BY CreatedAt DESC, TableNumber ASC";
            using var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
                orders.Add(ReadOrder(reader));
            return orders;
        }

        public void Update(OrderTable order)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"UPDATE TableOrder SET TableNumber = @tablenumber, TotalPrice = @total, PaymentID = @payment, OrderStatus = @status, ClosedAt = @closedAt WHERE TableOrderID = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@tablenumber", order.TableNumber);
            cmd.Parameters.AddWithValue("@total", order.TotalPrice);
            cmd.Parameters.AddWithValue("@payment", order.PaymentID);
            cmd.Parameters.AddWithValue("@status", (int)order.OrderStatus);
            cmd.Parameters.AddWithValue("@closedAt", (object?)order.ClosedAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", order.TableOrderID);
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

        private DateTime? GetDateTime(SqlDataReader r, string col)
        {
            return r[col] == DBNull.Value ? null : (DateTime)r[col];
        }

        private OrderTable ReadOrder(SqlDataReader reader)
        {
            return new OrderTable
            {
                TableOrderID = GetInt(reader, "TableOrderID"),
                TableNumber = (int)reader["TableNumber"],
                TotalPrice = GetDecimal(reader, "TotalPrice"),
                PaymentID = GetInt(reader, "PaymentID"),
                OrderStatus = (OrderStatus)GetInt(reader, "OrderStatus"),
                CreatedAt = (DateTime)reader["CreatedAt"],
                ClosedAt = GetDateTime(reader, "ClosedAt")
            };
        }

        public void UpdateStatus(int orderId, OrderStatus status)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"UPDATE TableOrder SET OrderStatus = @status WHERE TableOrderID = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", (int)status);
            cmd.Parameters.AddWithValue("@id", orderId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void CloseOrder(int orderId, decimal totalPrice, DateTime closedAt)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"UPDATE TableOrder SET ClosedAt = @closedAt, TotalPrice = @total WHERE TableOrderID = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@closedAt", closedAt);
            cmd.Parameters.AddWithValue("@total", totalPrice);
            cmd.Parameters.AddWithValue("@id", orderId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void ReopenOrder(int orderId)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"UPDATE TableOrder SET ClosedAt = NULL WHERE TableOrderID = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", orderId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}