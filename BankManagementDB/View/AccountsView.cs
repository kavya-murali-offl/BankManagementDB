using System;
using System.Linq;
using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Models;
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

                //Console.WriteLine("Choose Account type: \n1. Current Account \n2. Savings Account\n3. Go Back\n");
                //Console.WriteLine("Enter your choice: ");
                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);

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
