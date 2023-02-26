using BankManagementDB.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using BankManagementDB.View;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.EnumerationType;
using BankManagementDB.Config;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;

namespace BankManagementDB.View
{
    public class DashboardView
    {

        public DashboardView() {
            AccountController = DependencyContainer.ServiceProvider.GetRequiredService<IAccountController>();
            CustomerController = DependencyContainer.ServiceProvider.GetRequiredService<ICustomerController>();
            TransactionProcessingController = DependencyContainer.ServiceProvider.GetRequiredService<ITransactionProcessController>();
        }  

        public ITransactionProcessController TransactionProcessingController { get; set; }   
        public IAccountController AccountController { get; set; }

        public ICustomerController CustomerController { get; set; }

        public Customer CurrentUserController { get; set; } 


        public void ViewDashboard(Customer currentUser)
        {
            AccountController.GetAllAccounts(currentUser.ID);
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
                        if (DashboardOperations(cases, currentUser))
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
            Customer currentUser
            )
        {

            
            switch (operation)
            {
                case DashboardCases.PROFILE_SERVICES:
                    ProfileView profileView = new ProfileView(CustomerController);
                    profileView.ViewProfileServices();
                    return false;

                case DashboardCases.CREATE_ACCOUNT:
                    CreateAccount();
                    return false;

                case DashboardCases.LIST_ACCOUNTS:
                    ListAllAccounts(); 
                    return false;

                case DashboardCases.GO_TO_ACCOUNT:
                    GoToAccount();
                    return false;

                case DashboardCases.CARD_SERVICES:
                    GoToCardServices(currentUser);
                    return false;

                case DashboardCases.SIGN_OUT:
                    SaveCustomerSession();
                    return true;

                default:
                    Notification.Error("Enter a valid option.\n");
                    return false;
            }
        }

        public void GoToCardServices(Customer currentUser)
        {
            CardView cardsView = new CardView();
            cardsView.ShowCards(currentUser.ID);
        }

        public void CreateAccount()
        {

            AccountView accountsView = new AccountView();
            Account account = accountsView.GenerateAccount();
            if (account != null)
            {
                account.UserID = CustomerController.GetCurrentUser().ID;
                InsertAccount(account);
            }
        }

        public void InsertAccount(Account account)
        {
            bool inserted = AccountController.InsertAccount(account);
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
            while (true)
            {
                decimal amount = GetAmount();
                if (amount == 0)
                    return false;
                else if (amount > account.MinimumBalance)
                    if (TransactionProcessingController.Deposit(amount, account, ModeOfPayment.CASH))
                        return true;
                    else
                        Notification.Error("Initial deposit was not done. Try again");
                else
                    Notification.Error($"Initial deposit amount must be greater than Minimum Balance (Rs. {account.MinimumBalance}). Try again");
            }
        }

        public decimal GetAmount()
        {
            while (true)
            {
                Console.Write("Enter amount: ");
                try
                {
                    decimal amount = Decimal.Parse(Console.ReadLine().Trim());
                    if (amount > 0) return amount;
                    else Console.WriteLine("Amount should be greater than zero.");
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid amount. Try Again!");
                }
            }
        }

        public void GoToAccount()
        {
            try
            {
                while (true)
                {
                    TransactionView transactionView = new TransactionView();
                    Account transactionAccount = ChooseAccountForTransaction();

                    if (transactionAccount != null)
                        transactionView.GoToAccount(transactionAccount);
                    else
                        break;
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Account ChooseAccountForTransaction()
        {
            try
            {
                int accountIndex;

                IList<Account> accountsList = AccountController.GetAccountsList();

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
            IList<Account> accountsList = AccountController.GetAccountsList();

            foreach(Account account in accountsList) {
                Console.WriteLine(account);
            }
        }

        private void SaveCustomerSession()
        {
            CustomerController.UpdateCustomer(CustomerController.GetCurrentUser());
        }

        public void ListAccountIDs(IList<Account> accounts)
        {
            for(int i = 0; i < accounts.Count(); i++)
                Notification.Info(i+1 + ". " + accounts[i].AccountNumber);
        }
    }
}
