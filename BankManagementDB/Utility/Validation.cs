using System;
using System.Globalization;
using System.Text.RegularExpressions;
using BankManagementDB.View;

namespace BankManagement.Utility
{
    public class Validation
    {
        public bool CheckEmpty(string field)
        {
            if (field == null || field.Equals(""))
            {
                Notification.Warning("Field should not be empty");
                return false;
            }
            return true;
        }

        public bool ValidatePassword(string password, string rePassword)
        {
                return rePassword.Equals(password);
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();

                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
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
    }
}
