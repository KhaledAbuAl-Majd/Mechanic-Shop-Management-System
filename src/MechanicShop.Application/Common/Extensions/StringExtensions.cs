namespace MechanicShop.Application.Common.Extensions
{
    public static class StringExtensions
    {
        public static string MaskEmail(this string email)
        {
            int atIndex = email.IndexOf('@');
            if (atIndex <= 1)
            {
                return $"****{email.AsSpan(atIndex)}";
            }

            return email[0] + "****" + email[atIndex - 1] + email[atIndex..];
        }
    }
}
