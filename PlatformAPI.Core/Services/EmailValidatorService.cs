using System.Text.RegularExpressions;

namespace PlatformAPI.Core.Services
{
    public class EmailValidatorService
    {
        // List of valid email domains
        private static readonly string[] ValidProviders = {
            "gmail.com",
            "outlook.com",
            "hotmail.com",  
            "yahoo.com",
            "icloud.com",
            "aol.com",
            "zoho.com",
            "gmx.com",
            "protonmail.com",
            "mail.com",
            "yandex.com"
        };

        public static bool IsValidEmailProvider(string email)
        {
            // Validate the email format
            if (!IsValidEmailFormat(email))
            {
                return false;
            }

            // Extract the domain part of the email
            string domain = email.Substring(email.LastIndexOf('@') + 1);

            // Check if the domain is in the list of valid providers
            foreach (var provider in ValidProviders)
            {
                if (domain.Equals(provider, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsValidEmailFormat(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use regular expression to validate email format
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, emailPattern);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
