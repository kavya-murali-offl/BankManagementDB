using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
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

        public int GetInteger()
        {
            while (true)
            {
                try
                {
                    int number = int.Parse(Console.ReadLine().Trim());
                    if (number > 0) return number;
                    else Console.WriteLine("Enter a valid number.");
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid number. Try Again!");
                    GetInteger();   
                }
            }
        }

        public decimal GetAmount(CurrentAccount currentAccount)
        {
            while (true)
            {
                Console.WriteLine("Enter amount: ");
                try
                {
                    decimal amount = Decimal.Parse(Console.ReadLine().Trim());
                    if (amount > 0)
                    {
                        if(amount < currentAccount.MinimumBalance)
                        {
                            Console.WriteLine("Initial Amount should be greater than Minimum Balance.");
                        }
                        else
                        {
                            return amount;
                        }
                    }
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

        public int CountDays()
        {
            TransactionController transactionController = new TransactionController();
            DateTime? lastDepositDate = transactionController.GetLastDepositDate();
            if (lastDepositDate.HasValue)
            {
                int numberOfDays = (int)(DateTime.Now - lastDepositDate)?.TotalDays;
                return numberOfDays;
            }
            return 0;
        }
    }
}
