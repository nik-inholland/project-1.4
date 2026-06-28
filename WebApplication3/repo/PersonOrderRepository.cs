using Microsoft.Data.SqlClient;
using WebApplication3.Models;
using WebApplication3.repo.@interface;

namespace WebApplication3.repo
{
    public class PersonOrderRepository : IPersonOrderRepository
    {
        private readonly string _connectionString;

        public PersonOrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauConnection");
        }

        public List<OrderItem> GetOrderItemsByPersonOrderId(int personOrderId)
        {
            var items = new List<OrderItem>();

            string query = @"SELECT OrderItemID, itemName, Comments, PricePerItem, vat_category, Category, Quantity, MenuItemId, TimePlaced, PersonOrderId
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

        public List<PersonOrder> GetPersonOrdersByTable(OrderTable tableOrder)
        {
            List<PersonOrder> persons = new();

            using SqlConnection connection = new SqlConnection(_connectionString);

            string query = @"SELECT PersonOrderID, TableOrderID, PersonName, TotalPrice, PaymentID, OrderStatus, CreatedAt FROM PersonOrder WHERE TableOrderID = @id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", tableOrder.TableOrderID);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                persons.Add(ReadPersonOrder(reader));
            }

            return persons;
        }

        public void Update(PersonOrder personOrder)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"UPDATE PersonOrder SET TotalPrice = @total, PaymentID = @payment, OrderStatus = @status WHERE PersonOrderID = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@total", personOrder.TotalPrice);
            cmd.Parameters.AddWithValue("@payment", personOrder.PaymentID);
            cmd.Parameters.AddWithValue("@status", (int)personOrder.OrderStatus);
            cmd.Parameters.AddWithValue("@id", personOrder.PersonOrderID);
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

        private string? GetString(SqlDataReader r, string col)
        {
            return r[col] == DBNull.Value ? "" : r[col].ToString();
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

        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            var menuItem = new MenuItem
            {
                MenuItemID = GetInt(reader, "MenuItemID"),
                Description = GetString(reader, "itemName") ?? "",
                Price = GetDecimal(reader, "PricePerItem"),
                VatCategory = reader["vat_category"] != DBNull.Value && (bool)reader["vat_category"],
                CourseType = GetInt(reader, "Category"),
                QuantityInStock = GetInt(reader, "StockQuantity")
            };

            return new OrderItem
            {
                OrderItemID = GetInt(reader, "OrderItemID"),
                OrderID = GetInt(reader, "OrderID"),
                MenuItemID = menuItem.MenuItemID,
                MenuItem = menuItem,
                Quantity = GetInt(reader, "Quantity"),
                Comments = GetString(reader, "Comments"),
                itemStatus = (OrderStatus)GetInt(reader, "ItemStatus"),
                PlacedAt = (DateTime)reader["TimePlaced"]
            };
        }
    }
}
