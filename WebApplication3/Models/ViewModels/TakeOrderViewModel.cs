using System.Collections.Generic;
using System.Linq;
using WebApplication3.Models;

namespace WebApplication3.Models.ViewModels
{
    public class TakeOrderViewModel
    {
        public int TableNumber { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public List<OrderItem> CurrentOrderItems { get; set; } = new List<OrderItem>();
        public string SelectedCard { get; set; } = "All";

        public decimal TotalPrice => CurrentOrderItems.Sum(item => item.PricePerItem * item.Quantity);

        public TakeOrderViewModel() { }

        public TakeOrderViewModel(int tableNumber)
        {
            TableNumber = tableNumber;
        }

        public void AddItem(MenuItem menuItem, int quantity = 1, string comments = "")
        {
            if (menuItem == null) return;

            var existing = CurrentOrderItems.FirstOrDefault(i => i.MenuItemID == menuItem.MenuItemID);

            if (existing != null)
            {
                existing.Quantity += quantity;
                if (!string.IsNullOrEmpty(comments))
                    existing.Comments = comments;
            }
            else
            {
                CurrentOrderItems.Add(new OrderItem
                {
                    MenuItemID = menuItem.MenuItemID,
                    ItemName = menuItem.Description,
                    PricePerItem = menuItem.Price,
                    Quantity = quantity,
                    Comments = comments
                });
            }
        }

        public void RemoveItem(int menuItemID, bool removeAll = false)
        {
            var item = CurrentOrderItems.FirstOrDefault(i => i.MenuItemID == menuItemID);
            if (item == null) return;

            if (removeAll || item.Quantity == 1)
            {
                CurrentOrderItems.Remove(item);
            }
            else
            {
                item.Quantity--;
            }
        }

        public void ClearOrder()
        {
            CurrentOrderItems.Clear();
        }
    }
}