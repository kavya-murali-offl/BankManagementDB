using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using BankManagement.Controller;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.Controller;
using BankManagementDB.db;
using BankManagementDB.Interface;
using BankManagementDB.View;

namespace BankManagement.View
{
    public enum AccountCases
    {
        DEPOSIT, WITHDRAW, TRANSFER, CHECK_BALANCE, VIEW_STATEMENT, PRINT_STATEMENT, BACK
    }

    public class TransactionsView
    {

        public void GoToAccount(Account account)
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
        public void onBalanceChanged(string message)
        {
            Notification.Success(message);
        }

        public bool TransactionOperations(AccountCases option, Account account)
        {
            Helper helper = new Helper();
            ITransactionServices transactionController = new TransactionController(new TransactionOperations());
            IATMTransactionServices transactionATMController = new ATMTransactionsController(transactionController);

            transactionATMController.BalanceChanged += onBalanceChanged;
            decimal amount;

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
                    ViewAllTransactions();
                    return false;

                case AccountCases.PRINT_STATEMENT:
                    PrintStatement();
                    return false;

                case AccountCases.BACK:
                    return true;

                default:
                    Notification.Error("Invalid option. Try again!");
                    return false;
            }
        }

        public void Deposit(Account account, IATMTransactionServices transactionATMController)
        {
            Helper helper = new Helper();

            decimal amount = helper.GetAmount();
            transactionATMController.Deposit(amount, account);
        }

        public void Withdraw(Account account, IATMTransactionServices transactionATMController)
        {
            Helper helper = new Helper();

            decimal amount = helper.GetAmount();
            if (amount > account.Balance) Notification.Error("Insufficient Balance");
            else transactionATMController.Withdraw(amount, account);
        }

        public void Transfer(Account account, IATMTransactionServices transactionATMController)
        {
            Helper helper = new Helper();

            decimal amount = helper.GetAmount();
            if (amount > account.Balance) Notification.Error("Insufficient Balance");
            else
            {
                Guid transferAccountID = GetTransferAccountID(account.ID);
                transactionATMController.Transfer(amount, account, transferAccountID);
            }
        }

        public void ViewAllTransactions()
        {
            TransactionController transactionController = new TransactionController();  
            IList<Transaction> statements = transactionController.GetAllTransactions();
            foreach (Transaction transaction in statements)
                Console.WriteLine(transaction);
        }

        public void PrintStatement()
        {
            TransactionController transactionController = new TransactionController();
            IList<Transaction> statements = transactionController.GetAllTransactions();
            Printer.PrintStatement(statements);
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
    }
}
