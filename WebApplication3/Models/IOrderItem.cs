namespace WebApplication3.Models
{
    public interface IOrderItem
    {
        int MenuItemID { get; set; }

        string Description { get; set; }

        double Price { get; set; }

        bool VatCategory { get; set; }

        int CourseType { get; set; }

        int Quantity { get; set; }
    }
}