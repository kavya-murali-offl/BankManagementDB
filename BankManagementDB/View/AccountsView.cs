using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;

namespace BankManagement.View
{
    public class AccountsView
    { 
        public Account GenerateAccount()
        {
            Account account = null;
            while (true)
            {
                Console.WriteLine("Choose Account type: \n1. Current Account \n2. Savings Account\n3. Go Back");
                Console.WriteLine("Enter your choice: ");
                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);

                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(AccountTypes)).Count())
                    {
                        AccountTypes accountType = (AccountTypes)entryOption - 1;
                        account = AccountFactory.GetAccountByType(accountType);
                        if(account != null)
                        {
                            return account;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid option.");
                }
            }
            return account;
        }

        
    }

}
