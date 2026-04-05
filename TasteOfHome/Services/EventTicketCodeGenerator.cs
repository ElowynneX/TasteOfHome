using System.Security.Cryptography;

namespace TasteOfHome.Services
{
    public static class EventTicketCodeGenerator
    {
        public static string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(12);
            var token = Convert.ToHexString(bytes);
            return $"TOH-EVT-{token}";
        }
    }
}