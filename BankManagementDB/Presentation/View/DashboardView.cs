using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Models;
using System.Collections.Generic;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.EnumerationType;
using BankManagementDB.Config;
using BankManagementDB.Properties;
using BankManagementDB.Data;
using BankManagementDB.Utility;

namespace BankManagementDB.View
{

    public class DashboardView
    {
        public void ViewDashboard()
        {
            try
            {
                IGetAccountDataManager GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();
                GetAccountDataManager.GetAllAccounts(Store.CurrentUser.ID);
                OptionsDelegate<DashboardCases> options = DashboardOperations;

                HelperView helperView = new HelperView();
                helperView.PerformOperation(options);

            }
            catch (Exception error)
            {
                Notification.Error(error.ToString());
            }
        }

        public bool DashboardOperations(DashboardCases command) =>
        command switch
        {
            DashboardCases.PROFILE_SERVICES => GoToProfileServices(),
            DashboardCases.CREATE_ACCOUNT => CreateAccount(),
            DashboardCases.LIST_ACCOUNTS => ListAllAccounts(),
            DashboardCases.ACCOUNT_SERVICES => GoToAccount(),
            DashboardCases.CARD_SERVICES => GoToCardServices(),
            DashboardCases.EXIT => Exit(),
            DashboardCases.SIGN_OUT => Signout(),
            _ => Default()
        };


        private bool Default()
        {
            Notification.Error(Resources.InvalidOption);
            return false;
        }

        private bool Exit()
        {
            Environment.Exit(0);
            return true;
        }

        public bool GoToProfileServices()
        {
            ProfileView profileView = new ProfileView();
            profileView.ViewProfileServices();
            return false;
        }

        public bool Signout()
        {
            SaveCustomerSession();
            Store.CurrentUser = null;
            Notification.Success(Resources.LogoutSuccess);
            return true;
        }

        public bool GoToCardServices()
        {
            CardView cardsView = new CardView();
            cardsView.ShowCards();
            return false;
        }

        public bool CreateAccount()
        {

            AccountView accountsView = new AccountView();
            Account account = accountsView.GenerateAccount();
            if (account != null)
            {
                account.UserID = Store.CurrentUser.ID;
                InsertAccount(account);
            }

            return false;
        }

        private void InsertAccount(Account account)
        {
            IInsertAccountDataManager InsertAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertAccountDataManager>();
            bool inserted = InsertAccountDataManager.InsertAccount(account);
            if (inserted)
            {
                if (account is CurrentAccount)
                {
                    AccountView accountView = new AccountView();
                    AccountView.SelectedAccount = account;
                    while (true)
                    {
                        HelperView helperView = new HelperView();
                        decimal amount = helperView.GetAmount();
                        if (amount == 0)
                        {
                            break;
                        }
                        else if (amount > account.MinimumBalance)
                        {
                            accountView.Deposit(amount, ModeOfPayment.CASH, null);
                            break;
                        }
                        else
                        {
                            Notification.Warning(Formatter.FormatString(Resources.InitialDepositAmountWarning, account.MinimumBalance));
                        }
                    }
                }
                Notification.Success(Resources.AccountInsertSuccess);
            }
            else
            {
                Notification.Error(Resources.AccountInsertFailure);
            }
        }



        private bool GoToAccount()
        {
            IGetCardDataManager GetCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCardDataManager>();
            GetCardDataManager.GetAllCards(Store.CurrentUser.ID);
            while (true)
            {
                AccountView accountView = new AccountView();
                Account transactionAccount = ChooseAccountForTransaction();

                if (transactionAccount != null)
                {
                    accountView.GoToAccount(transactionAccount);
                }
                break;
            }
            return false;

        }

        public Account ChooseAccountForTransaction()
        {
            int accountIndex;
            IEnumerable<Account> accountsList = Store.AccountsList;

            if (accountsList.Count() == 1)
            {
                accountIndex = 1;
            }
            else
            {
                while (true)
                {
                    ListAccountIDs(accountsList);
                    Notification.Info(Resources.ChooseAccount);
                    Notification.Info(Resources.PressBackButtonInfo);
                    string index = Console.ReadLine()?.Trim();
                    Console.WriteLine();
                    if (!int.TryParse(index, out accountIndex))
                    { Notification.Error(Resources.InvalidInteger); }
                    else if (accountIndex > accountsList.Count())
                    { Notification.Error(Resources.ChooseOnlyFromOptions); }
                    else if (accountIndex <= accountsList.Count())
                    { break; }
                }
            }

            if (accountIndex > 0)
            {
                return accountsList.First();
            }

            return null;
        }

        private bool ListAllAccounts()
        {
            foreach (Account account in Store.AccountsList)
            {
                Console.WriteLine(account);
            }

            return false;
        }

        private void SaveCustomerSession()
        {
            IUpdateCustomerDataManager updateCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateCustomerDataManager>();
            updateCustomerDataManager.UpdateCustomer(Store.CurrentUser);
        }

        public void ListAccountIDs(IEnumerable<Account> accounts)
        {
            for (int i = 0; i < accounts.Count(); i++)
            {
                Notification.Info(i + 1 + ". " + accounts.ElementAt(i).AccountNumber);
            }
        }
    }
}
