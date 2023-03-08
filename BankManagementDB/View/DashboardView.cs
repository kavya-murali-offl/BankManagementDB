using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Models;
using System.Collections.Generic;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.EnumerationType;
using BankManagementDB.Config;
using BankManagementDB.Controller;
using BankManagementDB.Data;

namespace BankManagementDB.View
{
    public class DashboardView
    {

        public DashboardView() {
            GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();
            InsertAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertAccountDataManager>();
        }

        public IGetAccountDataManager GetAccountDataManager { get; private set; }

        public IInsertAccountDataManager InsertAccountDataManager { get; private set; }

        public object CurrentUserController { get; private set; }

        public void ViewDashboard()
        {
            try
            {
                GetAccountDataManager.GetAllAccounts(CacheData.CurrentUser.ID);
                while (true)
                {
                    for (int i = 0; i < Enum.GetNames(typeof(DashboardCases)).Length; i++)
                    {
                        DashboardCases cases = (DashboardCases)i;
                        Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                    }

                    Console.Write("\nEnter your choice: ");

              
                    string option = Console.ReadLine().Trim();
                    int entryOption;

                    if (!int.TryParse(option, out entryOption))
                        Notification.Error("Invalid input! Please enter a valid number.");
                    else
                    {
                        if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(DashboardCases)).Count())
                        {
                            DashboardCases cases = (DashboardCases)entryOption - 1;
                            if (DashboardOperations(cases))
                                break;
                        }
                        else
                            Notification.Error("Invalid input! Please enter a valid number.");
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        private bool DashboardOperations(DashboardCases operation)
        {
            switch (operation)
            {
                case DashboardCases.PROFILE_SERVICES:
                    ProfileView profileView = new ProfileView();
                    profileView.ViewProfileServices();
                    return false;

                case DashboardCases.CREATE_ACCOUNT:
                    CreateAccount();
                    return false;

                case DashboardCases.LIST_ACCOUNTS:
                    ListAllAccounts(); 
                    return false;

                case DashboardCases.ACCOUNT_SERVICES:
                    GoToAccount();
                    return false;

                case DashboardCases.CARD_SERVICES:
                    GoToCardServices();
                    return false;

                case DashboardCases.SIGN_OUT:
                    SaveCustomerSession();
                    CacheData.CurrentUser = null;
                    Notification.Success("User logged out");
                    return true;

                default:
                    Notification.Error("Enter a valid option.\n");
                    return false;
            }
        }

        public void GoToCardServices()
        {
            CardView cardsView = new CardView();
            cardsView.ShowCards();
        }

        public void CreateAccount()
        {

            AccountView accountsView = new AccountView();
            Account account = accountsView.GenerateAccount();
            if (account != null)
            {
                account.UserID = CacheData.CurrentUser.ID;
                InsertAccount(account);
            }
        }

        public void InsertAccount(Account account)
        {
            bool inserted = InsertAccountDataManager.InsertAccount(account);
            if (inserted)
            {
                if (account is CurrentAccount)
                    DepositAmount(account);
            }

            if (account != null)
                Notification.Success("Account created successfully");
            else
                Notification.Error("Account not created");
        }

        public bool DepositAmount(Account account)
        {
            TransactionView transactionView = new TransactionView();
            while (true)
            {
                decimal amount = GetAmount();
                if (amount == 0)
                    return false;
                else if (amount > account.MinimumBalance)
                {
                    transactionView.Deposit(account, amount, ModeOfPayment.CASH, null);
                    return true;
                }
                else
                    Notification.Error($"Initial deposit amount must be greater than Minimum Balance (Rs. {account.MinimumBalance}). Try again");
            }
        }

        public decimal GetAmount()
        {
            try
            {
                while (true)
                {       
                    Console.Write("Enter amount: ");
               
                    decimal amount = decimal.Parse(Console.ReadLine().Trim());
                    if (amount > 0) return amount;
                    else Console.WriteLine("Amount should be greater than zero.");
                }
             }
            catch (Exception error)
            {
                Notification.Error("Enter a valid amount. Try Again!");
            }
            return 0;
        }

        public void GoToAccount()
        {
            try
            {
                IGetCardDataManager GetCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCardDataManager>();
                GetCardDataManager.GetAllCards(CacheData.CurrentUser.ID);
                while (true)
                {
                    TransactionView transactionView = new TransactionView();
                    Account transactionAccount = ChooseAccountForTransaction();

                    if (transactionAccount != null)
                        transactionView.GoToAccount(transactionAccount);
                    break;
                }
            }catch(Exception ex)
            {
               Notification.Error(ex.ToString());
            }
        }

        public Account ChooseAccountForTransaction()
        {
            try
            {
                int accountIndex;
                IList<Account> accountsList = GetAccountDataManager.GetAllAccounts(CacheData.CurrentUser.ID);

                if (accountsList.Count() == 1)
                    accountIndex = 1;
                else
                {
                    while (true)
                    {
                        ListAccountIDs(accountsList);

                        Console.WriteLine("Choose the account or Press 0 to go back!\n");
                        string index = Console.ReadLine().Trim();

                        if (!int.TryParse(index, out accountIndex))
                            Notification.Error("Please enter a valid number.");
                        else if (accountIndex > accountsList.Count())
                            Notification.Error("Choose from the listed accounts.");
                        else if (accountIndex <= accountsList.Count())
                            break;
                    }
                }

                if(accountIndex > 0) 
                    return accountsList[accountIndex - 1];

            }
            catch(Exception e) {
                Console.WriteLine(e);
            }
            return null;
        }

        public void ListAllAccounts()
        {
            try
            {
                IList<Account> accountsList = GetAccountDataManager.GetAllAccounts(CacheData.CurrentUser.ID);

                foreach (Account account in accountsList)
                    Console.WriteLine(account);
            }
            catch(Exception ex)
            {
                Notification.Error(ex.Message);
            }
        }

        private void SaveCustomerSession()
        {
            try
            {
                IUpdateCustomerDataManager updateCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateCustomerDataManager>();
                updateCustomerDataManager.UpdateCustomer(CacheData.CurrentUser);
            }
            catch(Exception ex)
            {
                Notification.Error(ex.ToString());
            }
        }

        public void ListAccountIDs(IList<Account> accounts)
        {
            for(int i = 0; i < accounts.Count(); i++)
                Notification.Info(i+1 + ". " + accounts[i].AccountNumber);
        }
    }
}
