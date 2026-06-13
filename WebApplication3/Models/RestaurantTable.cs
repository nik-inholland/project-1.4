namespace WebApplication3.Models
{
    public class RestaurantTable
    {
        public int TableID { get; set; }

        public TableStatus Occupied { get; set; }

        public RestaurantTable()
        {
        }

        public RestaurantTable(
            int tableID,
            TableStatus occupied)
        {
            TableID = tableID;
            Occupied = occupied;
        }
    }
}