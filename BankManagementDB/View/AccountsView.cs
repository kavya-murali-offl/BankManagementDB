using System;
using System.Linq;
using System.Runtime.InteropServices;
using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.View;

namespace BankManagement.View
{
    public class AccountsView
    { 
        public Account GenerateAccount()
        {
            Account account = null;
            while (true)
            {
                Console.WriteLine("Choose Account Type: ");

                for (int i = 0; i < Enum.GetNames(typeof(AccountTypes)).Length; i++)
                {
                    AccountTypes cases = (AccountTypes)i;
                    Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                }

                Console.Write("\nEnter your choice: ");

                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    Helper helper = new Helper();
                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(AccountTypes)).Count())
                    {
                        AccountTypes accountType = (AccountTypes)entryOption - 1;
                        account = AccountFactory.GetAccountByType(accountType);

                        if(account != null)
                            return account;
                    }

                    else if(entryOption == 3)
                        break;

                    else
                    {
                        Notification.Error("Please enter a valid option");
                        continue;
                    }
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid option.");
                }
            }
            return account;
        }

        
    }

}
