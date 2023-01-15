using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace BankManagement.View
{
    public class AccountsView


    {
        public void ViewAllAccounts(IList<Account> accounts)
        {
            if (accounts == null || accounts.Count() == 0)
            {
                Console.WriteLine("\n-------- NO ACCOUNTS FOUND --------\n");
            }
            else
            {
                foreach (Account account in accounts)
                {
                    Console.WriteLine(account);
                }
            }
        }

        public void AccountSelection(ProfileController profile)
        {
            bool isValidOption = false;
            while (!isValidOption)
            {
                Console.WriteLine("Enter your choice: ");
                try
                {
                    string option = Console.ReadLine();
                    int entryOption = int.Parse(option);
                   
                    if (entryOption != 0 && entryOption <= profile.Accounts.Count())
                    {
                        Account account = profile.Accounts[entryOption - 1];
                    }
                    else
                    {
                        Console.WriteLine("Enter proper input.");
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid option(Linked Account view)");
                }
            }
        }

        public Account GenerateAccount()
        {
            Account account;
            while (true)
            {
                Console.WriteLine("Choose Account type: \n1. Current Account \n2. Savings Account\n3. Go Back");
                Console.WriteLine("Enter your choice: ");
                try
                {
                    string option = Console.ReadLine();
                    int entryOption = int.Parse(option);

                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(AccountTypes)).Count())
                    {
                        AccountTypes accountType = (AccountTypes)entryOption - 1;
                        account = AccountFactory.CreateAccountByType(accountType);
                        if (account != null) 
                            return account;
                    }
                    else
                        Console.WriteLine("Enter proper input.");
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid option.");
                }
            }
        }

        public void AccountCreatedSuccessMessage()
        {
            Console.WriteLine("Account created Successfully");
        }
    }
}
