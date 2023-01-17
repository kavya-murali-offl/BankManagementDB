using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagement.Utility
{
    public class Helper
    {
        public decimal GetAmount()
        {
            while (true)
            {
                Console.WriteLine("Enter amount: ");
                try
                {
                    decimal amount = Decimal.Parse(Console.ReadLine().Trim());
                    if (amount > 0) return amount;
                    else Console.WriteLine("Amount should be greater than zero.");
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid amount. Try Again!");
                }
            }
        }

        public String GetPhoneNumber()
        {
            Console.WriteLine("Enter Mobile Number: ");
            string phoneNumber = Console.ReadLine().Trim();
            Validation validation = new Validation();
            if (!validation.IsPhoneNumber(phoneNumber))
            {
                Console.WriteLine("Please enter a valid mobile number. ");
                GetPhoneNumber();
            }
            return phoneNumber;
        }

        public String GetPassword()
        {
            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine().Trim();
            Validation validation = new Validation();
            if (validation.CheckEmpty(password))
                return password;
            return null;
        }
    }
}
