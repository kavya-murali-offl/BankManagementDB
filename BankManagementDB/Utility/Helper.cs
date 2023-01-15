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
                    decimal amount = Decimal.Parse(Console.ReadLine());
                    if (amount > 0) return amount;
                    else Console.WriteLine("Amount should be greater than zero.");
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid amount. Try Again!(incoming view)");
                }
            }
        } 
        
        public bool CheckUniqueUserName(string userName)
        {
            CustomersController customersController = new CustomersController();
            return customersController.GetUserByUserName(userName) == null ? true : false;
        }

        public AccountTypes ConvertStringToAccountType(string accountType) { 
             AccountTypes enumType = AccountTypes.SAVINGS;
             switch(accountType)
            {
                case "CurrentAccount":
                    enumType = AccountTypes.CURRENT;
                    break;

                case "SavingsAccount":
                    enumType = AccountTypes.SAVINGS;
                    break;
            }

            return enumType;
        }

    }
}
