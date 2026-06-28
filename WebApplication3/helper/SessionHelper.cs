using System.Text.Json;
using WebApplication3.Models.ViewModels;

namespace WebApplication3.Helpers
{
    public static class SessionHelper
    {
        public static void SetTakeOrderViewModel(this ISession session, TakeOrderViewModel vm)
        {
            var json = JsonSerializer.Serialize(vm);
            session.SetString("CurrentOrder", json);
        }

        public static TakeOrderViewModel GetTakeOrderViewModel(this ISession session, int tableNumber)
        {
            var json = session.GetString("CurrentOrder");
            if (string.IsNullOrEmpty(json))
            {
                return new TakeOrderViewModel(tableNumber);
            }

            try
            {
                var vm = JsonSerializer.Deserialize<TakeOrderViewModel>(json);
                return vm ?? new TakeOrderViewModel(tableNumber);
            }
            catch
            {
                return new TakeOrderViewModel(tableNumber);
            }
        }

        public static void ClearCurrentOrder(this ISession session)
        {
            session.Remove("CurrentOrder");
        }
    }
}