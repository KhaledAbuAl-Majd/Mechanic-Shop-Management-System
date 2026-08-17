namespace MechanicShop.Application.Features.Customers.Constants
{
    public static class CustomerCache
    {
        public const string Tag = "customers";
        public const string AllKey = "customers:all";
        public static string ByIdKey(Guid id) => ByIdKey(id.ToString());
        public static string ByIdKey(string id) => $"customers:{id}";
    }
}
