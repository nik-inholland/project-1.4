namespace WebApplication3.helper
{
    public static class RoleHelper
    {
        public static string GetRole(HttpContext context)
        {
            return context.Session.GetString("EmployeeType") ?? "";
        }

        public static bool IsLoggedIn(HttpContext context)
        {
            return !string.IsNullOrEmpty(GetRole(context));
        }

        public static bool IsAdmin(HttpContext context)
        {
            var role = GetRole(context);
            return role == "admin" || role == "manager";
        }

        public static bool IsChef(HttpContext context)
        {
            var role = GetRole(context);
            return role == "chef" || role == "bartender";
        }

        public static bool IsWaiter(HttpContext context)
        {
            return GetRole(context) == "waiter";
        }

        public static bool CanEditOrders(HttpContext context)
        {
            var role = GetRole(context);
            return role == "admin" || role == "manager" || role == "chef" || role == "bartender";
        }

        public static bool CanServe(HttpContext context)
        {
            var role = GetRole(context);
            return role == "admin" || role == "waiter";
        }
    }
}
