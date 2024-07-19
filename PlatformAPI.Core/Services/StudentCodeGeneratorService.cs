using System.Text;

namespace PlatformAPI.Core.Services
{
    public class StudentCodeGeneratorService
    {
        private static readonly Random _random = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateCode(int length = 14)
        {
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(_chars[_random.Next(_chars.Length)]);
            }
            return result.ToString();
        }
    }
}
