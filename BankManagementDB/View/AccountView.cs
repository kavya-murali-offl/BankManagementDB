using System;
using System.Linq;
using BankManagementDB.EnumerationType;
using BankManagementDB.Models;
using BankManagementDB.Config;
using BankManagementDB.Interface;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Properties;

namespace BankManagementDB.View
{
    public class AccountView
    {
        public AccountView()
        {
            AccountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();
            GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();
        }

        public IGetAccountDataManager GetAccountDataManager { get; private set; }

        public IAccountFactory AccountFactory { get;private set; }

        public Account GenerateAccount()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine(Resources.ChooseAccountType);

                    for (int i = 0; i < Enum.GetNames(typeof(AccountType)).Length; i++)
                    {
                        AccountType cases = (AccountType)i;
                        Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                    }

                    Console.Write(Resources.EnterChoice);
                    string option = Console.ReadLine().Trim();

                    if (int.TryParse(option, out int entryOption))
                    {

                        if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(AccountType)).Count())
                        {
                            AccountType accountType = (AccountType)entryOption - 1;
                            Account account = AccountFactory.GetAccountByType(accountType);
                            if (account != null)
                                return account;
                        }
                        else
                            Notification.Error(Resources.InvalidOption);
                    }
                    else
                        Notification.Error(Resources.InvalidInteger);
                }
            }
            catch (Exception error)
            {
                Notification.Error(error.ToString());
            }
            return null;
        }

        public Account GetAccount()
        {
            try
            {
                while (true)
                {
                    Console.Write(Resources.EnterAccountNumber);
                    string accountNumber = Console.ReadLine().Trim();
                    if (accountNumber == Resources.BackButton)
                        break;
                    else
                    {
                        Account account = GetAccountDataManager.GetAccount(accountNumber);
                        if (account == null)
                            Notification.Error(Resources.InvalidAccountNumber);
                        else
                            return account;
                    }
                }
            }
            catch (Exception error)
            {
                Notification.Error(error.ToString());
            }
            return null;
        }
    }
}
