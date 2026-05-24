using Microsoft.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.repo
{
    public class TableRepository : ITableRepository
    {
        private readonly string _connectionString;

        public TableRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection");
        }

        public List<RestaurantTable> GetAll()
        {
            List<RestaurantTable> tables = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"SELECT tableID,
                         Occupied
                  FROM Tables
                  ORDER BY tableID";

            SqlCommand command =
                new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                tables.Add(ReadTable(reader));
            }

            return tables;
        }

        public RestaurantTable? GetById(int id)
        {
            RestaurantTable? table = null;

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"SELECT tableID,
                         Occupied
                  FROM Tables
                  WHERE tableID = @id";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                table = ReadTable(reader);
            }

            return table;
        }

        public void Update(RestaurantTable table)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            string query =
                @"UPDATE Tables
                  SET Occupied = @occupied
                  WHERE tableID = @id";

            SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@occupied",
                (int)table.Occupied);

            command.Parameters.AddWithValue(
                "@id",
                table.TableID);

            connection.Open();

            command.ExecuteNonQuery();
        }

        private RestaurantTable ReadTable(
            SqlDataReader reader)
        {
            return new RestaurantTable(
                (int)reader["tableID"],
                (TableStatus)(int)reader["Occupied"]);
        }
    }
}