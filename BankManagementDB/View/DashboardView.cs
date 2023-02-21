using BankManagement.Models;
using BankManagement.Controller;
using System.Collections.Generic;
using System;
using System.Linq;
using BankManagementDB.View;
using BankManagementDB.Interface;
using BankManagement.Model;
using BankManagement.Utility;
using BankManagementDB.Controller;
using BankManagementDB.db;
using BankManagementDB.Enums;

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
            TransactionController transactionController = new TransactionController(new TransactionOperations());
            AccountsController accountsController = new AccountsController(new AccountOperations()); 
            IATMTransactionServices transactionATMController = new ATMTransactionsController(transactionController, accountsController);
            
            switch (operation)
            {
                case DashboardCases.PROFILE_SERVICES:
                    ProfileView profileView = new ProfileView();
                    profileView.ViewProfileServices(profileController);
                    return false;

                case DashboardCases.CREATE_ACCOUNT:
                    CreateAccount(accountsController, transactionATMController, profileController);
                    return false;

                case DashboardCases.LIST_ACCOUNTS:
                    ListAllAccounts(accountsController); 
                    return false;

                case DashboardCases.GO_TO_ACCOUNT:
                    GoToAccount(accountsController, transactionATMController, transactionController);
                    return false;

                case DashboardCases.SIGN_OUT:
                    SaveCustomerSession(profileController);
                    return true;

                default:
                    Notification.Error("Enter a valid option.\n");
                    return false;
            }
        }

        public Account CreateAccount(IAccountServices accountsController, IATMTransactionServices transactionATMController, ProfileController profile)
        {

            AccountsView accountsView = new AccountsView();
            Account account = accountsView.GenerateAccount();
            account.UserID = profile.Customer.ID;
            bool inserted = accountsController.InsertAccount(account);
            if (inserted)
            {
                if (account is CurrentAccount)
                    if (!CheckAmount(account, transactionATMController)) 
                        Notification.Error("Initial Deposit unsuccessful");
            }

            if (account != null)
                Notification.Success("Account created successfully");
            else
                Notification.Error("Account not created");

            return account;
        }

        public bool CheckAmount(Account account, IATMTransactionServices transactionATMController)
        {
            Helper helper = new Helper();
            while (true)
            {
                decimal amount = helper.GetAmount();

                if (amount > account.MinimumBalance)
                    if (transactionATMController.Deposit(amount, account, ModeOfPayment.CASH))
                        return true;
                    else
                        Notification.Error("Initial deposit was not done. Try again");
                else
                    Notification.Error($"Initial deposit amount must be greater than Minimum Balance (Rs. {account.MinimumBalance}). Try again");
            }
            return false;
        }

        public void GoToAccount(AccountsController accountController,IATMTransactionServices transactionATMController, ITransactionServices transactionServices)
        {
            while (true)
            {
                TransactionsView transactionView = new TransactionsView();
                Account transactionAccount = ChooseAccountForTransaction(accountController);
                if (transactionAccount != null)
                {
                    transactionServices.FillTable(transactionAccount.ID);
                    transactionView.GoToAccount(transactionAccount, transactionATMController);
                }
                else
                    break;
            }
        }

        public Account ChooseAccountForTransaction(AccountsController accountController)
        {
            try
            {
                int accountIndex;

                IList<Account> accountsList = accountController.GetAccountsList();
                if (accountsList.Count() == 1)
                    accountIndex = 1;
                else
                {
                    ListAccountIDs(accountsList);

                    Console.WriteLine("Choose the account or Press 0 to go back!\n");
                    string index = Console.ReadLine().Trim();

                    if (!int.TryParse(index, out accountIndex))
                        Notification.Error("Please enter a valid number.");
                    else if (accountIndex == 0)
                        return null;
                    else if (accountIndex > accountsList.Count)
                    {
                        Notification.Error("Choose from the listed accounts.");
                        ChooseAccountForTransaction(accountController);
                    }
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
            IList<Account> accountsList = accountsController.GetAccountsList();

            foreach(Account account in accountsList) {
                Console.WriteLine(account);
            }
        }

        private void SaveCustomerSession(ProfileController profileController)
        {
            CustomerOperations customerOperations = new CustomerOperations();
            CustomersController customersController = new CustomersController(customerOperations, customerOperations);
            customersController.UpdateCustomer(profileController.Customer);
        }

        public void ListAccountIDs(IList<Account> accounts)
        {
            for(int i = 0; i < accounts.Count(); i++)
                Notification.Info(i+1 + ". " + accounts[i].AccountNumber);
        }
    }
}
