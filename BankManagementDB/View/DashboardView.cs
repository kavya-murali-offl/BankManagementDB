using BankManagement.Models;
using BankManagement.Controller;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BankManagement.View
{
    public enum DashboardCases
    {
        PROFILE,
        CREATE_ACCOUNT,
        LIST_ACCOUNTS,
        GO_TO_ACCOUNT,
        SIGN_OUT
    }

    public class DashboardView
    {
        public void ViewDashboard(ProfileController profileController)
        {
            while (true)
            {
                Console.WriteLine($@"
                      1. Profile
                    \n2. Create Account
                    \n3. List Accounts
                    \n4. Go to Account
                    \n5. Sign out
                    \nEnter your choice: ");
                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(DashboardCases)).Count())
                    {
                        DashboardCases cases = (DashboardCases)entryOption - 1;

                        if (DashboardOperations(cases, profileController))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Enter proper input.");
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

        private bool DashboardOperations(
            DashboardCases operation,
            ProfileController profileController
            )
        {
            AccountsController accountsController = new AccountsController();   
            switch (operation)
            {
                case DashboardCases.PROFILE:
                    ProfileView profileView = new ProfileView();
                    profileView.GetProfileDetails(profileController);
                    return false;
                case DashboardCases.CREATE_ACCOUNT:
                    bool isCreated = accountsController.CreateAccount(profileController.ID);
                    if (isCreated) Console.WriteLine("Successfully created account");
                    else Console.WriteLine("Account not created");
                    return false;
                case DashboardCases.LIST_ACCOUNTS:
                    ListAllAccounts(accountsController); 
                    return false;
                case DashboardCases.GO_TO_ACCOUNT:
                    GoToAccount(accountsController);
                    return false;
                case DashboardCases.SIGN_OUT:
                    Console.WriteLine(".....LOGGING YOU OUT.....");
                    CustomersController customersController = new CustomersController();    
                    IDictionary<string, object> updateFields = new Dictionary<string, object>();
                    updateFields.Add("lastLoginOn", profileController.lastLoginOn);
                    //customersController.UpdateCustomer(updateFields);
                    return true;
                default:
                    Console.WriteLine("Enter a valid option.\n");
                    return false;
            }
        }

        public void GoToAccount(AccountsController accountController)
        {
           
            TransactionsView transactionView = new TransactionsView();
            Account transactionAccount = ChooseAccountForTransaction(accountController);
            TransactionController transactionController = new TransactionController();
            transactionController.FillTable(transactionAccount.ID);
            transactionView.GoToAccount(transactionAccount, accountController);
        }

        public Account ChooseAccountForTransaction(AccountsController accountController)
        {
            IList<Account> accountsList = accountController.GetAllAccounts();
            ListAccountIDs(accountsList);
            string index = Console.ReadLine().Trim();
            int accountIndex = int.Parse(index);
            return accountsList[accountIndex-1];
        }

        public void ListAllAccounts(AccountsController accountsController)
        {
            IList<Account> accountsList = accountsController.GetAllAccounts();
            foreach(Account account in accountsList) {
                Console.WriteLine(account);
            }
        }

        public void ListAccountIDs(IList<Account> accounts)
        {
            for(int i = 1; i<accounts.Count()+1;i++)
            {
                Console.WriteLine(i + ". " + accounts[i - 1].ID);
            }
        }

    }
}
