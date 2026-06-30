using Microsoft.Data.SqlClient;
using WebApplication3.Models;
using WebApplication3.repo.@interface;

namespace WebApplication3.repo
{
    public class BarKitchenRepository : IBarKitchenRepository
    {
        private readonly string _connectionString;

        public BarKitchenRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauConnection");
        }


        public List<OrderTable> GetOpenOrdersWithItems(int minCourse, int maxCourse)
        {
            string query = @"
                SELECT  o.TableOrderID, o.TableNumber, o.TotalPrice, o.PaymentID,
                        o.OrderStatus, o.CreatedAt, o.ClosedAt,
                        oi.OrderItemID, oi.OrderID, oi.menuItemID, oi.Quantity,
                        oi.Comments, oi.ItemStatus, oi.TimePlaced,
                        mi.description, mi.price, mi.course_type
                FROM    TableOrder o
                INNER JOIN OrderItems oi ON oi.OrderID = o.TableOrderID
                INNER JOIN MenuItem  mi ON mi.menuItemID = oi.menuItemID
                WHERE   o.ClosedAt IS NULL
                  AND   mi.course_type BETWEEN @min AND @max
                ORDER BY o.CreatedAt ASC, o.TableOrderID, mi.course_type";

            return ReadOrdersWithItems(query, minCourse, maxCourse);
        }

        public List<OrderTable> GetTodaysOrdersWithItems(int minCourse, int maxCourse)
        {
            string query = @"
                SELECT  o.TableOrderID, o.TableNumber, o.TotalPrice, o.PaymentID,
                        o.OrderStatus, o.CreatedAt, o.ClosedAt,
                        oi.OrderItemID, oi.OrderID, oi.menuItemID, oi.Quantity,
                        oi.Comments, oi.ItemStatus, oi.TimePlaced,
                        mi.description, mi.price, mi.course_type
                FROM    TableOrder o
                INNER JOIN OrderItems oi ON oi.OrderID = o.TableOrderID
                INNER JOIN MenuItem  mi ON mi.menuItemID = oi.menuItemID
                WHERE   CONVERT(date, o.CreatedAt) = CONVERT(date, GETDATE())
                  AND   mi.course_type BETWEEN @min AND @max
                ORDER BY o.CreatedAt DESC, o.TableOrderID, mi.course_type";

            return ReadOrdersWithItems(query, minCourse, maxCourse);
        }

        private List<OrderTable> ReadOrdersWithItems(string query, int minCourse, int maxCourse)
        {
            var ordersById = new Dictionary<int, OrderTable>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@min", minCourse);
            command.Parameters.AddWithValue("@max", maxCourse);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int orderId = reader.GetInt32(reader.GetOrdinal("TableOrderID"));

                if (!ordersById.TryGetValue(orderId, out var order))
                {
                    order = new OrderTable
                    {
                        TableOrderID = orderId,
                        TableNumber = reader.GetInt32(reader.GetOrdinal("TableNumber")),
                        TotalPrice = reader.IsDBNull(reader.GetOrdinal("TotalPrice"))
                            ? 0m : reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                        PaymentID = reader.GetInt32(reader.GetOrdinal("PaymentID")),
                        orderStatus = (OrderStatus)reader.GetInt32(reader.GetOrdinal("OrderStatus")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        ClosedAt = reader.IsDBNull(reader.GetOrdinal("ClosedAt"))
                            ? null : reader.GetDateTime(reader.GetOrdinal("ClosedAt"))
                    };
                    ordersById.Add(orderId, order);
                }

                var menuItem = new MenuItem
                {
                    MenuItemID = reader.GetInt32(reader.GetOrdinal("menuItemID")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description"))
                        ? "" : reader.GetString(reader.GetOrdinal("description")),
                    Price = reader.IsDBNull(reader.GetOrdinal("price"))
                        ? 0m : reader.GetDecimal(reader.GetOrdinal("price")),
                    CourseType = reader.GetInt32(reader.GetOrdinal("course_type"))
                };

                order.OrderItems.Add(new OrderItem
                {
                    OrderItemID = reader.GetInt32(reader.GetOrdinal("OrderItemID")),
                    OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                    MenuItemID = menuItem.MenuItemID,
                    MenuItem = menuItem,
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Comments = reader.IsDBNull(reader.GetOrdinal("Comments"))
                        ? null : reader.GetString(reader.GetOrdinal("Comments")),
                    itemStatus = (OrderStatus)reader.GetInt32(reader.GetOrdinal("ItemStatus")),
                    PlacedAt = reader.IsDBNull(reader.GetOrdinal("TimePlaced"))
                        ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("TimePlaced"))
                });
            }

            return ordersById.Values.ToList();
        }


        public void UpdateOrderStatus(int orderId, OrderStatus status, int minCourse, int maxCourse)
        {
            string query = @"
                UPDATE TableOrder SET OrderStatus = @status WHERE TableOrderID = @id;

                UPDATE OrderItems SET ItemStatus = @status
                WHERE OrderID = @id
                  AND menuItemID IN (
                        SELECT menuItemID FROM MenuItem
                        WHERE course_type BETWEEN @min AND @max);";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@status", (int)status);
            command.Parameters.AddWithValue("@id", orderId);
            command.Parameters.AddWithValue("@min", minCourse);
            command.Parameters.AddWithValue("@max", maxCourse);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void UpdateOrderItemStatus(int orderItemId, OrderStatus status)
        {
            string query = "UPDATE OrderItems SET ItemStatus = @status WHERE OrderItemID = @id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@status", (int)status);
            command.Parameters.AddWithValue("@id", orderItemId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void UpdateItemsStatusByCourseTypes(int orderId, int[] courseTypes, OrderStatus status)
        {
            if (courseTypes == null || courseTypes.Length == 0) return;

            
            var paramNames = new string[courseTypes.Length];
            for (int i = 0; i < courseTypes.Length; i++)
                paramNames[i] = "@c" + i;
            string inList = string.Join(",", paramNames);

            string query = $@"
                UPDATE OrderItems SET ItemStatus = @status
                WHERE OrderID = @id
                  AND menuItemID IN (
                        SELECT menuItemID FROM MenuItem
                        WHERE course_type IN ({inList}));";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@status", (int)status);
            command.Parameters.AddWithValue("@id", orderId);
            for (int i = 0; i < courseTypes.Length; i++)
                command.Parameters.AddWithValue(paramNames[i], courseTypes[i]);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
