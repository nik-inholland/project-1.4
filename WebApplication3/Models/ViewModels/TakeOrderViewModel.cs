using WebApplication3.Models;

namespace WebApplication3.Models.ViewModels
{
    public class TakeOrderViewModel
    {
        public int TableNumber { get; set; }
        public IEnumerable<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public List<OrderItem> CurrentOrderItems { get; set; } = new List<OrderItem>();

        public decimal TotalPrice => CurrentOrderItems?.Sum(i => i.Quantity * i.MenuItem?.Price ?? 0) ?? 0m;

        public TakeOrderViewModel() { }

        public TakeOrderViewModel(int tableNumber)
        {
            TableNumber = tableNumber;
        }

        public void AddItem(MenuItem menuItem, int quantity, string? comments = null)
        {
            if (menuItem == null) throw new ArgumentNullException(nameof(menuItem));
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));

            var existing = CurrentOrderItems.FirstOrDefault(i => i.MenuItemID == menuItem.MenuItemID);
            if (existing != null)
            {
                existing.Quantity += quantity;
                if (!string.IsNullOrEmpty(comments))
                    existing.Comments = comments;
            }
            else
            {
                var orderItem = new OrderItem(menuItem, quantity, comments);
                CurrentOrderItems.Add(orderItem);
            }
        }

        public void RemoveItem(int menuItemId, bool removeAll = true, int quantityToRemove = 1)
        {
            var existing = CurrentOrderItems.FirstOrDefault(i => i.MenuItemID == menuItemId);
            if (existing == null) return;

            if (removeAll || existing.Quantity <= quantityToRemove)
            {
                CurrentOrderItems.Remove(existing);
            }
            else
            {
                existing.Quantity -= quantityToRemove;
            }
        }

        public void ClearItems()
        {
            CurrentOrderItems.Clear();
        }
    }
}