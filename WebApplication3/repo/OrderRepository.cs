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
                configuration.GetConnectionString("DefaultConnection");
        }

        public OrderTable? GetById(int id)
        {
            OrderTable? order = null;

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"SELECT TableOrderID,
                         TableNumber,
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

        public List<OrderTable> GetAll()
        {
            List<OrderTable> orders = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"SELECT TableOrderID,
                         TableNumber,
                         TotalPrice,
                         PaymentID,
                         OrderStatus,
                         CreatedAt
                  FROM TableOrder
                  ORDER BY CreatedAt DESC";

            SqlCommand command =
                new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(ReadOrder(reader));
            }

            return orders;
        }

        public List<OrderTable> GetRunningOrders()
        {
            List<OrderTable> orders = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"SELECT TableOrderID,
                         TableNumber,
                         TotalPrice,
                         PaymentID,
                         OrderStatus,
                         CreatedAt
                  FROM TableOrder
                  WHERE OrderStatus <> @served
                  ORDER BY CreatedAt ASC";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@served",
                (int)OrderStatus.Served);

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(ReadOrder(reader));
            }

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

        public List<OrderItem> GetOrderItemsByOrderId(int tableOrderId)
        {
            List<OrderItem> items = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"SELECT OrderItemID,
                         PersonOrderID,
                         menuItemID,
                         itemName,
                         PricePerItem,
                         vat_category,
                         Category,
                         Quantity,
                         Comments,
                         ItemStatus
                  FROM OrderItems
                  WHERE PersonOrderID IN
                  (
                      SELECT PersonOrderID
                      FROM PersonOrder
                      WHERE TableOrderID = @tableOrderId
                  )
                  ORDER BY Category, itemName";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@tableOrderId",
                tableOrderId);

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(ReadOrderItem(reader));
            }

            return items;
        }

        public List<OrderTable> GetFinishedOrdersToday()
        {
            List<OrderTable> orders = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"SELECT TableOrderID,
                 TableNumber,
                 TotalPrice,
                 PaymentID,
                 OrderStatus,
                 CreatedAt
          FROM TableOrder
          WHERE OrderStatus = @served
          AND CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE)
          ORDER BY CreatedAt DESC";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@served",
                (int)OrderStatus.Served);

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(ReadOrder(reader));
            }

            return orders;
        }

        public void UpdateOrderStatus(int orderId, int status)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"UPDATE TableOrder
                  SET OrderStatus = @status
                  WHERE TableOrderID = @id";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", orderId);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public void UpdatePersonOrderStatus(int personOrderId, int status)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"UPDATE PersonOrder
                  SET OrderStatus = @status
                  WHERE PersonOrderID = @id";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", personOrderId);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public void UpdateOrderItemStatus(int orderItemId, int status)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"UPDATE OrderItems
                  SET ItemStatus = @status
                  WHERE OrderItemID = @id";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", orderItemId);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public void UpdateCourseStatus(int tableOrderId, int courseType, int status)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"UPDATE OrderItems
          SET ItemStatus = @status
          WHERE Category = @courseType
          AND PersonOrderID IN
          (
              SELECT PersonOrderID
              FROM PersonOrder
              WHERE TableOrderID = @tableOrderId
          )";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@courseType", courseType);
            command.Parameters.AddWithValue("@tableOrderId", tableOrderId);

            connection.Open();

            command.ExecuteNonQuery();
        }

        private OrderTable ReadOrder(SqlDataReader reader)
        {
            return new OrderTable
            {
                TableOrderID = GetInt(reader, "TableOrderID"),
                TableID = GetInt(reader, "TableNumber"),
                TotalPrice = GetDecimal(reader, "TotalPrice"),
                PaymentID = GetInt(reader, "PaymentID"),
                OrderStatus = (OrderStatus)GetInt(reader, "OrderStatus"),
                OrderDateTime = GetDateTime(reader, "CreatedAt")
            };
        }

        private PersonOrder ReadPersonOrder(SqlDataReader reader)
        {
            return new PersonOrder
            {
                PersonOrderID = GetInt(reader, "PersonOrderID"),
                TableOrderID = GetInt(reader, "TableOrderID"),
                PersonName = GetString(reader, "PersonName"),
                TotalPrice = GetDecimal(reader, "TotalPrice"),
                PaymentID = GetInt(reader, "PaymentID"),
                OrderStatus = (OrderStatus)GetInt(reader, "OrderStatus"),
                CreatedAt = GetDateTime(reader, "CreatedAt")
            };
        }

        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            return new OrderItem
            {
                OrderItemID = GetInt(reader, "OrderItemID"),
                TableOrderID = GetInt(reader, "PersonOrderID"),
                MenuItemID = GetInt(reader, "menuItemID"),
                Description = GetString(reader, "itemName"),
                Price = GetDouble(reader, "PricePerItem"),
                VatCategory = GetBool(reader, "vat_category"),
                CourseType = GetInt(reader, "Category"),
                Quantity = GetInt(reader, "Quantity"),
                Comment = GetString(reader, "Comments"),
                ItemStatus = (OrderStatus)GetInt(reader, "ItemStatus")
            };
        }

        private int GetInt(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? 0
                : Convert.ToInt32(reader[column]);
        }

        private decimal GetDecimal(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? 0m
                : Convert.ToDecimal(reader[column]);
        }

        private double GetDouble(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? 0
                : Convert.ToDouble(reader[column]);
        }

        private bool GetBool(SqlDataReader reader, string column)
        {
            return reader[column] != DBNull.Value
                && Convert.ToBoolean(reader[column]);
        }

        private DateTime GetDateTime(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? DateTime.MinValue
                : Convert.ToDateTime(reader[column]);
        }

        private string GetString(SqlDataReader reader, string column)
        {
            return reader[column] == DBNull.Value
                ? ""
                : reader[column].ToString();
        }
    }
}