using System;
using System.Linq;
using BankManagementDB.EnumerationType;
using BankManagementDB.Models;
using BankManagementDB.Config;
using BankManagementDB.Interface;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Controller;
using BankManagementDB.Model;
using BankManagementDB.DataManager;

namespace BankManagementDB.View
{
    public class AccountView
    {
        public AccountView()
        {
            AccountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();
            GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();
        }

        public IGetAccountDataManager GetAccountDataManager { get;private set; }
        public IAccountFactory AccountFactory { get;private set; }

        public Account GenerateAccount()
        {
            try
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
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (entryOption == 0)
                        return null;
                    if (entryOption <= Enum.GetNames(typeof(AccountType)).Count())
                    {
                        AccountType accountType = (AccountType)entryOption - 1;
                        Account account = AccountFactory.GetAccountByType(accountType);
                        if (account != null)
                            return account;
                    }
                    else
                        Notification.Error("Please enter a valid option");
                }
            }
            catch (Exception error)
            {
                Notification.Error("Enter a valid option.");
            }
            return null;
        }

        public Account GetAccount()
        {
            try
            {
                while (true)
                {
                    Console.Write("Enter Account Number: ");
                    string accountNumber = Console.ReadLine().Trim();
                    if (accountNumber == "0")
                        break;
                    else
                    {
                        Account account = GetAccountDataManager.GetAccount(accountNumber);
                        if (account == null)
                            Notification.Error("Invalid Account Number");
                        else
                            return account;
                    }
                }
            }
            catch (Exception error)
            {
                Notification.Error("Enter a valid option.");
            }
            return null;
        }
    }
}
