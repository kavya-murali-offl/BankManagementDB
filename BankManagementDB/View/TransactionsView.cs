using System;
using System.Collections.Generic;
using System.Linq;
using BankManagement.Controller;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.Controller;
using BankManagementDB.Enums;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.View;

namespace BankManagement.View
{
    public enum AccountCases
    {
        DEPOSIT, WITHDRAW, TRANSFER, CHECK_BALANCE, VIEW_STATEMENT, PRINT_STATEMENT, VIEW_ACCOUNT_DETAILS, CARD_SERVICES, BACK
    }

    public enum MoneyServices
    {
        CASH, CREDIT_CARD, DEBIT_CARD
    }

    public class TransactionsView
    {

        public void GoToAccount(Account account, IATMTransactionServices transactionATMcontroller)
        {
            while (true)
            {
                for (int i = 0; i < Enum.GetNames(typeof(AccountCases)).Length; i++)
                {
                    AccountCases cases = (AccountCases)i;
                    Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                }

                Console.Write("\nEnter your choice: ");

                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(AccountCases)).Count())
                    {
                        AccountCases operation = (AccountCases)entryOption - 1;
                        if (TransactionOperations(operation, account, transactionATMcontroller))
                            break;
                    }
                    else
                        Notification.Error("Enter a valid input.");
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid option. Try Again!");
                }
            }
        }
       
        public bool TransactionOperations(AccountCases option, Account account, IATMTransactionServices transactionATMController)
        {

            switch (option)
            {
                case AccountCases.DEPOSIT:
                    Deposit(account, transactionATMController);
                    return false;

                case AccountCases.WITHDRAW:
                    Withdraw(account, transactionATMController);
                    return false;

                case AccountCases.TRANSFER:
                    Transfer(account, transactionATMController);
                    return false;

                case AccountCases.CHECK_BALANCE:
                    Notification.Info($"\nBALANCE: Rs. {account.Balance}\n");
                    return false;

                case AccountCases.VIEW_STATEMENT:
                    ViewAllTransactions(account.ID);
                    return false;

                case AccountCases.PRINT_STATEMENT:
                    PrintStatement(account.ID);
                    return false;

                case AccountCases.VIEW_ACCOUNT_DETAILS:
                    ViewAccountDetails(account);
                    return false;

                case AccountCases.CARD_SERVICES:
                    GoToCardServices(account);
                    return false;

                case AccountCases.BACK:
                    return true;

                default:
                    Notification.Error("Invalid option. Try again!");
                    return false;
            }

        }
        public void GoToCardServices(Account account)
        {
            CardsView cardsView = new CardsView();

            cardsView.ShowCards(account);
        }

        public ModeOfPayment? GetModeOfPayment(Guid id)
        {
            while (true)
            {

                for (int i = 0; i < Enum.GetNames(typeof(MoneyServices)).Length; i++)
                {
                    MoneyServices cases = (MoneyServices)i;
                    Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                }
                Console.WriteLine("Press 0 to go back!");
                Console.Write("\nEnter your choice: ");

                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if(entryOption == 0)
                    {
                        return null;
                    }
                    else if (entryOption <= Enum.GetNames(typeof(MoneyServices)).Count())
                    {
                        ModeOfPayment operation = (ModeOfPayment)entryOption - 1;
                        return operation;
                    }
                    else
                        Notification.Error("Enter a valid input.");
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid option. Try Again!");
                }
            }
        }

        public void Deposit(Account account, IATMTransactionServices transactionATMController)
        {
            Helper helper = new Helper();


            CardsView cardsView = new CardsView();
            ModeOfPayment? nullableMode = GetModeOfPayment(account.ID);

            if (nullableMode != null)
            {
                ModeOfPayment modeOfPayment = (ModeOfPayment)nullableMode;

                transactionATMController.BalanceChanged += onBalanceChanged;
                if (cardsView.ValidateModeOfPayment(account.ID, modeOfPayment))
                {

                    if (IsAuthenticated(modeOfPayment))
                    {
                        decimal amount = helper.GetAmount();
                        transactionATMController.Deposit(amount, account, modeOfPayment);
                    }
                    else
                        Notification.Error("Authentication failed. Please try again");
                }
                else
                    Notification.Error("Selected mode of payment is not enabled.");

                transactionATMController.BalanceChanged -= onBalanceChanged;
            }
        }



        public void Withdraw(Account account, IATMTransactionServices transactionATMController)
        {
            Helper helper = new Helper();
            CardsView cardsView = new CardsView();
            ModeOfPayment? nullableMode = GetModeOfPayment(account.ID);

            if (nullableMode != null)
            {
                ModeOfPayment modeOfPayment = (ModeOfPayment)nullableMode;

                transactionATMController.BalanceChanged += onBalanceChanged;

                if (cardsView.ValidateModeOfPayment(account.ID, modeOfPayment))
                {

                    if (IsAuthenticated(modeOfPayment))
                    {
                        decimal amount = helper.GetAmount();
                        if (amount > account.Balance) Notification.Error("Insufficient Balance");
                        else transactionATMController.Withdraw(amount, account, modeOfPayment);
                    }
                    else
                        Notification.Error("Authentication failed. Please try again");
                }

                transactionATMController.BalanceChanged -= onBalanceChanged;
            }
        }

        public void Transfer(Account account, IATMTransactionServices transactionATMController)
        {
            Helper helper = new Helper();

            ModeOfPayment? nullableMode = GetModeOfPayment(account.ID);
            if (nullableMode != null)
            {
                ModeOfPayment modeOfPayment = (ModeOfPayment)nullableMode;
                transactionATMController.BalanceChanged += onBalanceChanged;
                CardsView cardsView = new CardsView();
                if (cardsView.ValidateModeOfPayment(account.ID, modeOfPayment))
                {
                    if (IsAuthenticated(modeOfPayment))
                    {
                        decimal amount = helper.GetAmount();
                        if (amount > account.Balance) Notification.Error("Insufficient Balance");
                        else
                        {
                            Guid transferAccountID = GetTransferAccountID(account.ID);
                            transactionATMController.Transfer(amount, account, transferAccountID, modeOfPayment);
                        }
                    }
                    else
                        Notification.Error("Authentication failed. Please try again");
                }
                else
                    Notification.Error("Selected mode of payment is not enabled.");

                transactionATMController.BalanceChanged -= onBalanceChanged;
            }
        }

        public void ViewAllTransactions(Guid accountID)
        {
            ITransactionServices transactionController = new TransactionController();  
            IEnumerable<Transaction> statements = transactionController.GetAllTransactions(accountID);
            foreach (Transaction transaction in statements)
                Console.WriteLine(transaction);
        }

        public void PrintStatement(Guid accountID)
        {
            ITransactionServices transactionController = new TransactionController();
            IEnumerable<Transaction> statements = transactionController.GetAllTransactions(accountID);
            Printer.PrintStatement(statements);
        }

        public void ViewAccountDetails(Account account)
        {
            Console.WriteLine(account);
        }

        public Guid GetTransferAccountID(Guid ID)
        {
            while (true)
            {
                try
                {
                    Console.Write("Enter Account ID to transfer: ");
                    string id = Console.ReadLine().Trim();
                    Guid inputID = new Guid(id);
                    if (inputID.Equals(ID))
                    {
                        Notification.Error("Choose a different account number to transfer.");
                        continue;
                    }
                    return inputID;
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid ID.");
                }
            }
        }

        public void onBalanceChanged(string message)
        {
            Notification.Success(message);
        }

        public bool IsAuthenticated(ModeOfPayment modeOfPayment)
        {

            CardsView cardsView = new CardsView();
            if (modeOfPayment == ModeOfPayment.CASH)
               return true;
            else
               return cardsView.Authenticate();
            return false;
        }

    }
}
