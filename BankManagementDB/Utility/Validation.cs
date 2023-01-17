using System;

namespace BankManagement.Utility
{
    public class Validation
    {
        public bool CheckEmpty(string field)
        {
            if (field.Equals(""))
            {
                Console.WriteLine("Field should not be empty");
                return false;
            }
            return true;
        }

        public bool ValidatePassword(string password, string rePassword)
        {
                return rePassword.Equals(password);
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
