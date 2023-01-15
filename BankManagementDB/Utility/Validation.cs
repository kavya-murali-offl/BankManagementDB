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

    }
}
