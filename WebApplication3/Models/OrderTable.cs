using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication3.Models
{
    public class OrderTable
    {
        public int TableOrderID { get; set; }

        public int TableID { get; set; }

        public decimal TotalPrice { get; set; }

        public int PaymentID { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime OrderDateTime { get; set; }

        public List<PersonOrder> PersonOrders { get; set; } = new();

        public List<OrderItem> OrderItems { get; set; } = new();

        public List<OrderItem> Drinks
        {
            get
            {
                return OrderItems
                    .Where(item => item.CourseType == 0)
                    .ToList();
            }
        }

        public List<OrderItem> Starters
        {
            get
            {
                return OrderItems
                    .Where(item => item.CourseType == 1)
                    .ToList();
            }
        }

        public List<OrderItem> Mains
        {
            get
            {
                return OrderItems
                    .Where(item => item.CourseType == 2)
                    .ToList();
            }
        }

        public List<OrderItem> Desserts
        {
            get
            {
                return OrderItems
                    .Where(item => item.CourseType == 3)
                    .ToList();
            }
        }

        public string WaitingTimeText
        {
            get
            {
                if (OrderDateTime == DateTime.MinValue)
                {
                    return "0 min";
                }

                DateTime todayOrderTime =
                    DateTime.Today
                    .AddHours(OrderDateTime.Hour)
                    .AddMinutes(OrderDateTime.Minute);

                TimeSpan waitingTime =
                    DateTime.Now - todayOrderTime;

                if (waitingTime.TotalMinutes < 1)
                {
                    return "Less than 1 min";
                }

                int hours = (int)waitingTime.TotalHours;
                int minutes = waitingTime.Minutes;

                if (hours > 0)
                {
                    return hours + "h " + minutes + "min";
                }

                return minutes + " min";
            }
        }
    }
}