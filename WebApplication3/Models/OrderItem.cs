namespace WebApplication3.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }

        public int MenuItemID { get; set; }

        public int TableOrderID { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public bool VatCategory { get; set; }

        public int CourseType { get; set; }

        public int Quantity { get; set; }

        public string Comment { get; set; }

        public OrderStatus ItemStatus { get; set; }

        public string StatusText
        {
            get
            {
                if (ItemStatus == OrderStatus.Ordered)
                    return "Ordered";

                if (ItemStatus == OrderStatus.BeingPrepared)
                    return "Being Prepared";

                if (ItemStatus == OrderStatus.ReadyToBeServed)
                    return "Ready To Be Served";

                return "Served";
            }
        }

        public OrderItem()
        {
        }
    }
}