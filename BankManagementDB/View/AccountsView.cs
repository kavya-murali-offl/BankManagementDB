using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;

namespace BankManagement.View
{
    public class AccountsView
    {
        public void AccountSelection()
        {
            bool isValidOption = false;
            while (!isValidOption)
            {
                Console.WriteLine("Enter your choice: ");
                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    AccountsController accountsController = new AccountsController();
                    IList<Account> accountsList = accountsController.GetAllAccounts();
                    if (entryOption != 0 && entryOption <= accountsList.Count())
                    {
                        Account account = accountsList[entryOption - 1];
                    }
                    else
                    {
                        Console.WriteLine("Enter proper input.");
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid option.");
                }
            }
        }

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
                        Helper helper = new Helper();
                        if(account!=null)
                        {
                            if (account.Type == AccountTypes.CURRENT) { 
                                decimal amount = helper.GetAmount();
                                TransactionController transactionController = new TransactionController();
                                transactionController.Deposit(amount, account);
                            }
                            break;
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
