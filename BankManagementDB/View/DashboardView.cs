using BankManagement.Models;
using BankManagement.Controller;
using System.Collections.Generic;
using System;
using System.Linq;
using BankManagementDB.View;
using BankManagementDB.Interface;
using BankManagement.Model;
using BankManagement.Utility;

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
                Console.WriteLine("1. Profile Services\n2. Create Account\n3. List Accounts\n4. Go to Account\n5. Sign out\nEnter your choice: ");
                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption;

                    if (!int.TryParse(option, out entryOption))
                    {
                        Notification.Error("Invalid input! Please enter a valid number.");
                        continue;
                    }

                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(DashboardCases)).Count())
                    {
                        DashboardCases cases = (DashboardCases)entryOption - 1;
                        if (DashboardOperations(cases, profileController))
                            break;
                    }
                    else
                        Notification.Error("Invalid input! Please enter a valid number.");
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
                    profileView.ViewProfileServices(profileController);
                    return false;
                case DashboardCases.CREATE_ACCOUNT:
                    accountsController.CreateAccount(profileController.ID);
                    return false;
                case DashboardCases.LIST_ACCOUNTS:
                    ListAllAccounts(accountsController); 
                    return false;
                case DashboardCases.GO_TO_ACCOUNT:
                    GoToAccount(accountsController);
                    return false;
                case DashboardCases.SIGN_OUT:
                    SaveCustomerSession(profileController);
                    return true;
                default:
                    Notification.Error("Enter a valid option.\n");
                    return false;
            }
        }

        public void onBalanceChanged(string message)
        {
           Notification.Success(message);
        }

        public void GoToAccount(AccountsController accountController)
        {
            TransactionsView transactionView = new TransactionsView();
            Account transactionAccount = ChooseAccountForTransaction(accountController);
            transactionAccount.BalanceChanged += onBalanceChanged;
            TransactionController transactionController = new TransactionController();
            transactionController.FillTable(transactionAccount.ID);
            transactionView.GoToAccount(transactionAccount);
        }

        public Account ChooseAccountForTransaction(AccountsController accountController)
        {
            try
            {
                IList<Account> accountsList = accountController.GetAllAccounts();
                ListAccountIDs(accountsList);
                string index = Console.ReadLine().Trim();
                int accountIndex;
                if (!int.TryParse(index, out accountIndex))
                    Notification.Error("Please enter a valid number.");
                if (accountIndex  > accountsList.Count)
                {
                    Notification.Error("Choose from the listed accounts.");
                    ChooseAccountForTransaction(accountController);
                }
                return accountsList[accountIndex - 1];
            }
            catch(Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }

        public void ListAllAccounts(AccountsController accountsController)
        {
            IList<Account> accountsList = accountsController.GetAllAccounts();
            foreach(Account account in accountsList) {
                Console.WriteLine(account);
            }
        }

        private void SaveCustomerSession(ProfileController profileController)
        {
            CustomersController customersController = new CustomersController();
            IDictionary<string, object> updateFields = new Dictionary<string, object>
            {
                {"ID", profileController.ID },   
                { "LastLoggedOn", profileController.LastLoggedOn }
            };
            customersController.UpdateCustomer(updateFields);
        }

        public void ListAccountIDs(IList<Account> accounts)
        {
            for(int i = 1; i<accounts.Count()+1;i++)
                Console.WriteLine(i + ". " + accounts[i - 1].ID);
        }
    }
}
