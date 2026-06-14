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
        public void SaveOrder(OrderTable order, List<OrderItem> orderItems)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string insertOrder = @"INSERT INTO TableOrder (TableNumber, TotalPrice, PaymentID, OrderStatus, CreatedAt)
                           OUTPUT INSERTED.TableOrderID
                           VALUES (@tableNumber, @totalPrice, @paymentID, @orderStatus, @createdAt)";

            int tableOrderId;
            using (var cmd = new SqlCommand(insertOrder, conn))
            {
                cmd.Parameters.AddWithValue("@tableNumber", order.TableNumber);
                cmd.Parameters.AddWithValue("@totalPrice", order.TotalPrice);
                cmd.Parameters.AddWithValue("@paymentID", order.PaymentID);
                cmd.Parameters.AddWithValue("@orderStatus", (int)order.OrderStatus);
                cmd.Parameters.AddWithValue("@createdAt", order.CreatedAt);
                tableOrderId = (int)cmd.ExecuteScalar();
            }

            string insertPerson = @"INSERT INTO PersonOrder (TableOrderID, PersonName, TotalPrice, PaymentID, OrderStatus, CreatedAt)
                            OUTPUT INSERTED.PersonOrderID
                            VALUES (@tableOrderID, @personName, @totalPrice, @paymentID, @orderStatus, @createdAt)";

            int personOrderId;
            using (var cmd = new SqlCommand(insertPerson, conn))
            {
                cmd.Parameters.AddWithValue("@tableOrderID", tableOrderId);
                cmd.Parameters.AddWithValue("@personName", "Guest");
                cmd.Parameters.AddWithValue("@totalPrice", order.TotalPrice);
                cmd.Parameters.AddWithValue("@paymentID", 0);
                cmd.Parameters.AddWithValue("@orderStatus", (int)order.OrderStatus);
                cmd.Parameters.AddWithValue("@createdAt", order.CreatedAt);
                personOrderId = (int)cmd.ExecuteScalar();
            }

            string insertItem = @"INSERT INTO OrderItems (itemName, Comments, PricePerItem, vat_category, Category, Quantity, MenuItemId, TimePlaced, PersonOrderId)
                          VALUES (@name, @comments, @price, @vat, @category, @quantity, @menuItemId, @placedAt, @personOrderId)";

            foreach (var item in orderItems)
            {
                using var cmd = new SqlCommand(insertItem, conn);
                cmd.Parameters.AddWithValue("@name", item.ItemName);
                cmd.Parameters.AddWithValue("@comments", (object?)item.Comments ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@price", item.PricePerItem);
                cmd.Parameters.AddWithValue("@vat", item.VatCategory.HasValue && item.VatCategory.Value == 1);
                cmd.Parameters.AddWithValue("@category", (object?)item.Category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                cmd.Parameters.AddWithValue("@menuItemId", item.MenuItemID);
                cmd.Parameters.AddWithValue("@placedAt", item.PlacedAt ?? DateTime.Now);
                cmd.Parameters.AddWithValue("@personOrderId", personOrderId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}