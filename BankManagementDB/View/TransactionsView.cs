using BankManagement.Controller;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.Interface;
using BankManagementDB.View;
using System;
using System.Collections.Generic;
using System.Linq;

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
                Console.WriteLine("\n1. Deposit \n2. Withdraw\n3. Transfer\n4. Check Balance \n5. View Statement\n6. Print Statement\n7. Back \nEnter your choice: ");
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
            Helper helper = new Helper();
            ITransactionServices transactionController = new TransactionController();
            IStatementServices statementServices = new TransactionController();
            decimal amount;

            switch (option)
            {
                case AccountCases.DEPOSIT:
                    amount = helper.GetAmount();
                    transactionController.Deposit(amount, account);
                    return false;

                case AccountCases.WITHDRAW:
                    amount = helper.GetAmount();
                    transactionController.Withdraw(amount, account);
                    return false;

                case AccountCases.TRANSFER:
                    amount = helper.GetAmount();
                    long transferAccountID = GetTransferAccountID(account.ID);
                    transactionController.Transfer(amount, account, transferAccountID);
                    return false;

                case AccountCases.CHECK_BALANCE:
                    Notification.Info($"BALANCE: {account.Balance}");
                    return false;

                case AccountCases.VIEW_STATEMENT:
                    statementServices.ViewAllTransactions();
                    return false;

                case AccountCases.PRINT_STATEMENT:
                    IList<Transaction> statements = statementServices.GetAllTransactions();
                    Printer.PrintStatement(statements);
                    return false;

                case AccountCases.BACK:
                    return true;

                default:
                    Notification.Error("Invalid option. Try again!");
                    return false;
            }
        }

        public long GetTransferAccountID(long ID)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter Account ID to transfer: ");
                    string id = Console.ReadLine().Trim();
                    long intID = long.Parse(id);
                    if (intID == ID)
                    {
                        Notification.Error("Choose a different account number to transfer.");
                        continue;
                    }
                    return intID;
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid ID.");
                }
            }
        }
    }
}
