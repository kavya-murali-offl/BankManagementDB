using System;
using System.Text.RegularExpressions;

namespace BankManagementDB.Utility
{
    public class Validation
    {
        public bool ValidatePassword(string password, string rePassword) => rePassword.Equals(password);

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                    @"^[a-z0-9][-a-z0-9._]+@([-a-z0-9]+[.])+[a-z]{2,5}$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public bool IsValidPin(string input)
        {
            string pattern = "0000";
            bool matched = false;

            if (input.Length != pattern.Length) return false;

            for (int i = 0; i < input.Length; ++i)
            {
                matched = Char.IsDigit(input, i);
                if (!matched) return false;
            }
            return matched;
        }

        public bool IsPhoneNumber(string input)
        {
            string pattern = "0000000000";
            bool matched = false;

            if (input.Length != pattern.Length) return false;

            for (int i = 0; i < input.Length; ++i)
            {
                matched = Char.IsDigit(input, i);
                if (!matched) return false;
            }
            return matched;
        }

        public static bool IsValidGuid(string str) =>  Guid.TryParse(str, out Guid guid);
    }
}
