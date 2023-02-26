using System;
using System.Linq;
using System.Runtime.InteropServices;
using BankManagementDB.Controller;
using BankManagementDB.EnumerationType;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementDB.Config;
using BankManagementDB.Interface;
using BankManagementDB.View;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagementDB.View
{
    public class AccountView
    { 
        public Account GenerateAccount()
        {
            while (true)
            {
                Console.WriteLine("Choose Account Type: ");

                for (int i = 0; i < Enum.GetNames(typeof(AccountType)).Length; i++)
                {
                    AccountType cases = (AccountType)i;
                    Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                }

                Console.Write("\nEnter your choice: ");

                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (entryOption == 0)
                        return null;
                    if (entryOption <= Enum.GetNames(typeof(AccountType)).Count())
                    {
                        AccountType accountType = (AccountType)entryOption - 1;

                        IAccountFactory accountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();
                        Account account = accountFactory.GetAccountByType(accountType);
                        if(account != null)
                            return account;
                    }
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
        }
    }

}
