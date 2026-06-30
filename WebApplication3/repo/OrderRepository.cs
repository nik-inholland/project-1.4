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
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string orderQuery = @"
        SELECT TableOrderID, TableNumber, TotalPrice, PaymentID, OrderStatus, CreatedAt, ClosedAt
        FROM TableOrder
        WHERE TableOrderID = @id";

            using var cmd = new SqlCommand(orderQuery, connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            var order = ReadOrder(reader);
            reader.Close();

            order.OrderItems = GetOrderItems(id);

            return order;
        }


        public List<OrderItem> GetOrderItems(int orderId)
        {
            var items = new List<OrderItem>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"
                SELECT oi.OrderItemID, oi.TableNumber, oi.ItemName, oi.Quantity, 
                       oi.Comments, oi.ItemStatus, oi.menuItemID, 
                       oi.TimePlaced AS PlacedAt, oi.OrderID AS OrderID
                FROM OrderItems oi
                INNER JOIN MenuItem mi ON oi.menuItemID = mi.menuitemID
                WHERE oi.OrderID = @orderId 
                  AND mi.course_type < 8
                ORDER BY mi.course_type";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@orderId", orderId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(ReadOrderItem(reader));
            }
            return items;
        }

        public OrderItem? GetOrderItemById(int orderItemId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"
        SELECT OrderItemID, TableNumber, ItemName, Quantity, Comments, ItemStatus,
               menuItemID, TimePlaced AS PlacedAt, OrderID AS OrderID
        FROM OrderItems
        WHERE OrderItemID = @orderItemId";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@orderItemId", orderItemId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return ReadOrderItem(reader);
        }

        public List<OrderTable> GetAllTableOrders()
        {
            var orders = new List<OrderTable>();
            using var connection = new SqlConnection(_connectionString);
            string query = @"
                SELECT TableOrderID, TableNumber, TotalPrice, PaymentID, OrderStatus, CreatedAt, ClosedAt
                FROM TableOrder
                ORDER BY CreatedAt DESC, TableNumber ASC";

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
            string query = @"
                UPDATE TableOrder
                SET TableNumber = @tablenumber,
                    TotalPrice = @total,
                    PaymentID = @payment,
                    OrderStatus = @status,
                    ClosedAt = @closedAt
                WHERE TableOrderID = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@tablenumber", order.TableNumber);
            cmd.Parameters.AddWithValue("@total", order.TotalPrice);
            cmd.Parameters.AddWithValue("@payment", order.PaymentID);
            cmd.Parameters.AddWithValue("@status", (int)order.orderStatus);
            cmd.Parameters.AddWithValue("@closedAt", order.ClosedAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id", order.TableOrderID);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateStatus(int orderId, OrderStatus status)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = "UPDATE TableOrder SET OrderStatus = @status WHERE TableOrderID = @id" +
                " UPDATE OrderItems SET ItemStatus = @status WHERE OrderID = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", (int)status);
            cmd.Parameters.AddWithValue("@id", orderId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateOrderItemStatus(int orderItemId, OrderStatus newStatus)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = "UPDATE OrderItems SET ItemStatus = @status WHERE OrderItemID = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", (int)newStatus);
            cmd.Parameters.AddWithValue("@id", orderItemId);
            conn.Open();
          cmd.ExecuteNonQuery();
        }

        public void CloseOrder(int orderId, decimal totalPrice, DateTime closedAt)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"
                UPDATE TableOrder
                SET ClosedAt = @closedAt,
                    TotalPrice = @total,
                    OrderStatus = @status
                WHERE TableOrderID = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@closedAt", closedAt);
            cmd.Parameters.AddWithValue("@total", totalPrice);
            cmd.Parameters.AddWithValue("@status", (int)OrderStatus.Served);
            cmd.Parameters.AddWithValue("@id", orderId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void ReopenOrder(int orderId)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"
                UPDATE TableOrder
                SET ClosedAt = NULL,
                    OrderStatus = @status
                WHERE TableOrderID = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", (int)OrderStatus.Ordered);
            cmd.Parameters.AddWithValue("@id", orderId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void SaveOrder(OrderTable order)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string insertOrder = @"
                INSERT INTO TableOrder (TableNumber, TotalPrice, PaymentID, OrderStatus, CreatedAt)
                OUTPUT INSERTED.TableOrderID
                VALUES (@tableNumber, @totalPrice, @paymentID, @orderStatus, @createdAt)";

            int orderId;
            using (var cmd = new SqlCommand(insertOrder, conn))
            {
                cmd.Parameters.AddWithValue("@tableNumber", order.TableNumber);
                cmd.Parameters.AddWithValue("@totalPrice", order.TotalPrice);
                cmd.Parameters.AddWithValue("@paymentID", order.PaymentID);
                cmd.Parameters.AddWithValue("@orderStatus", (int)order.orderStatus);
                cmd.Parameters.AddWithValue("@createdAt", order.CreatedAt);
                orderId = (int)cmd.ExecuteScalar();
            }

            string insertItem = @"
                INSERT INTO OrderItems (OrderID, MenuItemID, Quantity, Comments, ItemStatus, TimePlaced)
                VALUES (@orderId, @menuItemId, @quantity, @comments, @itemStatus, @placedAt)";

            foreach (var item in order.OrderItems)
            {
                using var cmd = new SqlCommand(insertItem, conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@menuItemId", item.MenuItemID);
                cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                cmd.Parameters.AddWithValue("@comments", item.Comments ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@itemStatus", item.itemStatus);
                cmd.Parameters.AddWithValue("@placedAt", item.PlacedAt);
                cmd.ExecuteNonQuery();
            }
        }

        public List<OrderTable> GetOrdersByClosedState(bool closed, int? limit = null)
        {
            var orders = new List<OrderTable>();
            using var connection = new SqlConnection(_connectionString);
            string query = @"
                SELECT TableOrderID, TableNumber, TotalPrice, PaymentID, OrderStatus, CreatedAt, ClosedAt
                FROM TableOrder
                WHERE ClosedAt IS " + (closed ? "NOT NULL" : "NULL") + @"
                ORDER BY " + (closed ? "TableNumber ASC, ClosedAt DESC" : "CreatedAt DESC, TableNumber ASC");

            if (limit.HasValue)
                query += " OFFSET 0 ROWS FETCH NEXT @limit ROWS ONLY";

            using var cmd = new SqlCommand(query, connection);
            if (limit.HasValue)
                cmd.Parameters.AddWithValue("@limit", limit.Value);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                orders.Add(ReadOrder(reader));
            return orders;
        }

        private OrderTable ReadOrder(SqlDataReader reader)
        {
            return new OrderTable
            {
                TableOrderID = reader.GetInt32(reader.GetOrdinal("TableOrderID")),
                TableNumber = reader.GetInt32(reader.GetOrdinal("TableNumber")),
                TotalPrice = reader.IsDBNull(reader.GetOrdinal("TotalPrice")) ? 0m : reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                PaymentID = reader.GetInt32(reader.GetOrdinal("PaymentID")),
                orderStatus = (OrderStatus)reader.GetInt32(reader.GetOrdinal("OrderStatus")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                ClosedAt = reader.IsDBNull(reader.GetOrdinal("ClosedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ClosedAt"))
            };
        }
        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            return new OrderItem
            {
                OrderItemID = reader.GetInt32(reader.GetOrdinal("OrderItemID")),
                OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                MenuItemID = reader.GetInt32(reader.GetOrdinal("MenuItemID")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                itemStatus = (OrderStatus)reader.GetInt32(reader.GetOrdinal("ItemStatus")),
                PlacedAt = reader.GetDateTime(reader.GetOrdinal("PlacedAt"))
            };
        }

        public List<OrderTable> GetRecentTableOrders(int count, bool showClosed, string? dateFilter = null)
        {
            var orders = new List<OrderTable>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"
        SELECT TOP (@count) TableOrderID, TableNumber, TotalPrice, PaymentID, OrderStatus, CreatedAt, ClosedAt
        FROM TableOrder o
        WHERE (@showClosed = 1 OR ClosedAt IS NULL)
          AND (@dateFilter IS NULL OR CONVERT(date, CreatedAt) = @dateFilter)
          AND EXISTS (
              SELECT 1
              FROM OrderItems oi
              INNER JOIN MenuItem mi ON oi.menuItemID = mi.menuitemID
              WHERE oi.OrderID = o.TableOrderID
                AND mi.course_type < 8
          )
        ORDER BY CreatedAt DESC";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@count", count);
            cmd.Parameters.AddWithValue("@showClosed", showClosed ? 1 : 0);

            DateTime? filterDate = null;
            if (!string.IsNullOrEmpty(dateFilter) && DateTime.TryParse(dateFilter, out DateTime parsedDate))
                filterDate = parsedDate.Date;

            cmd.Parameters.AddWithValue("@dateFilter", filterDate.HasValue ? (object)filterDate.Value : DBNull.Value);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(ReadOrder(reader));
            }
            return orders;
        }
    }
}