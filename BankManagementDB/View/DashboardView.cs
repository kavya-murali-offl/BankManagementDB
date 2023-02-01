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
        PROFILE_SERVICES,
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
                for (int i = 0; i < Enum.GetNames(typeof(DashboardCases)).Length; i++)
                {
                    DashboardCases cases = (DashboardCases)i;
                    Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                }
                Console.Write("\nEnter your choice: ");

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
            ITransactionServices transactionController = new TransactionController();
            AccountsController accountsController = new AccountsController(transactionController);   
            switch (operation)
            {
                case DashboardCases.PROFILE_SERVICES:
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
            while (true)
            {
                TransactionsView transactionView = new TransactionsView();
                Account transactionAccount = ChooseAccountForTransaction(accountController);
                if (transactionAccount != null)
                {
                    transactionAccount.BalanceChanged += onBalanceChanged;
                    TransactionController transactionController = new TransactionController();
                    transactionController.FillTable(transactionAccount.ID);
                    transactionView.GoToAccount(transactionAccount);
                }
                else
                    break;
            }
        }

        public Account ChooseAccountForTransaction(AccountsController accountController)
        {
            try
            {
                IList<Account> accountsList = accountController.GetAllAccounts();
                ListAccountIDs(accountsList);
                Console.WriteLine("Press 0 to go back!\n");
                string index = Console.ReadLine().Trim();
                int accountIndex;

                if (!int.TryParse(index, out accountIndex))
                    Notification.Error("Please enter a valid number.");
                else if (accountIndex == 0)
                    return null;
                else if (accountIndex > accountsList.Count)
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
            IDictionary<string, dynamic> updateFields = new Dictionary<string, dynamic>
            {
                {"ID", profileController.ID },   
                { "LastLoggedOn", profileController.LastLoggedOn }
            };
            customersController.UpdateCustomer(updateFields);
        }

        public void ListAccountIDs(IList<Account> accounts)
        {
            for(int i = 1; i<accounts.Count()+1;i++)
                Notification.Info(i + ". " + accounts[i - 1].ID);
        }
    }
}
