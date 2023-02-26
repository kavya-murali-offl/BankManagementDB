using System;
using System.Collections.Generic;
using System.Linq;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.View;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagementDB.View
{
    public class TransactionView
    {
        public TransactionView() {
            TransactionProcessingController = DependencyContainer.ServiceProvider.GetRequiredService<ITransactionProcessController>(); ;
            TransactionController = DependencyContainer.ServiceProvider.GetRequiredService<ITransactionController>();
        }

        public ITransactionProcessController TransactionProcessingController { get; set; }

        public ITransactionController TransactionController { get; set; }

        public void GoToAccount(Account account)
        {

            TransactionController.FillTable(account.ID);
            while (true)
            {
                Console.WriteLine();
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
                        if (TransactionOperations(operation, account))
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
       
        public bool TransactionOperations(AccountCases option, Account account)
        {
            switch (option)
            {
                case AccountCases.DEPOSIT:
                    Deposit(account);
                    return false;

                case AccountCases.WITHDRAW:
                    Withdraw(account);
                    return false;

                case AccountCases.TRANSFER:
                    Transfer(account);
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

                case AccountCases.BACK:
                    return true;

                default:
                    Notification.Error("Invalid option. Try again!");
                    return false;
            }

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

        public void Deposit(Account account)
        {
            CardView cardsView = new CardView();
            ModeOfPayment? nullableMode = GetModeOfPayment(account.ID);

            if (nullableMode != null)
            {
                ModeOfPayment modeOfPayment = (ModeOfPayment)nullableMode;

                TransactionProcessingController.BalanceChanged += onBalanceChanged;

                if (cardsView.ValidateModeOfPayment(account.ID, modeOfPayment))
                {

                    if (IsAuthenticated(modeOfPayment))
                    {
                        decimal amount = GetAmount();
                        TransactionProcessingController.Deposit(amount, account, modeOfPayment);
                    }
                    else
                        Notification.Error("Authentication failed. Please try again");
                }
                else
                    Notification.Error("Selected mode of payment is not enabled.");

                TransactionProcessingController.BalanceChanged -= onBalanceChanged;
            }
        }



        public void Withdraw(Account account)
        {
            CardView cardsView = new CardView();
            ModeOfPayment? nullableMode = GetModeOfPayment(account.ID);

            if (nullableMode != null)
            {
                ModeOfPayment modeOfPayment = (ModeOfPayment)nullableMode;

                TransactionProcessingController.BalanceChanged += onBalanceChanged;

                if (cardsView.ValidateModeOfPayment(account.ID, modeOfPayment))
                {

                    if (IsAuthenticated(modeOfPayment))
                    {
                        decimal amount = GetAmount();
                        if (amount > account.Balance) Notification.Error("Insufficient Balance");
                        else TransactionProcessingController.Withdraw(amount, account, modeOfPayment);
                    }
                    else
                        Notification.Error("Authentication failed. Please try again");
                }

                TransactionProcessingController.BalanceChanged -= onBalanceChanged;
            }
        }

        public void Transfer(Account account)
        {
            ModeOfPayment? nullableMode = GetModeOfPayment(account.ID);
            if (nullableMode != null)
            {
                ModeOfPayment modeOfPayment = (ModeOfPayment)nullableMode;
                TransactionProcessingController.BalanceChanged += onBalanceChanged;
                CardView cardsView = new CardView();
                if (cardsView.ValidateModeOfPayment(account.ID, modeOfPayment))
                {
                    if (IsAuthenticated(modeOfPayment))
                    {
                        decimal amount = GetAmount();
                        if (amount > account.Balance) Notification.Error("Insufficient Balance");
                        else
                        {
                            Guid transferAccountID = GetTransferAccountID(account.ID);
                            if(transferAccountID != Guid.Empty)
                                TransactionProcessingController.Transfer(amount, account, transferAccountID, modeOfPayment);
                        }
                    }
                    else
                        Notification.Error("Authentication failed. Please try again");
                }
                else
                    Notification.Error("Selected mode of payment is not enabled.");

                TransactionProcessingController.BalanceChanged -= onBalanceChanged;
            }
        }

        public void ViewAllTransactions(Guid accountID)
        {
            IEnumerable<Transaction> statements = TransactionController.GetAllTransactions(accountID);
            foreach (Transaction transaction in statements)
                Console.WriteLine(transaction);
        }

        public void PrintStatement(Guid accountID)
        {
            IEnumerable<Transaction> statements = TransactionController.GetAllTransactions(accountID);
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
                    Guid inputID;
                    if (id == "0")
                        inputID = Guid.Empty;
                    else
                    {
                        inputID = new Guid(id);
                        if (inputID.Equals(ID))
                        {
                            Notification.Error("Choose a different account number to transfer.");
                            continue;
                        }
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

            CardView cardsView = new CardView();
            if (modeOfPayment == ModeOfPayment.CASH)
               return true;
            else
               return cardsView.Authenticate();
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
                    else Notification.Error("Amount should be greater than zero.");
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid amount. Try Again!");
                }
            }
        }

    }
}
