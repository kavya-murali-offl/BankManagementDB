

using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.Interface;
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
        public void GoToAccount(Account account, AccountsController accountsController)
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
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Enter proper input.");
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid option. Try Again!(view dashboard)");
                }
            }
        }

        public bool TransactionOperations(AccountCases option, Account account)
        {
            Helper helper = new Helper();
            ITransactionServices transactionController = new TransactionController();
            TransactionController statementServies = new TransactionController();
            decimal amount;
            switch (option)
            {
                case AccountCases.DEPOSIT:
                    amount = helper.GetAmount();
                    transactionController.Deposit(amount, account);
                    return true;
                case AccountCases.WITHDRAW:
                    amount = helper.GetAmount();
                    transactionController.Withdraw(amount, account);
                    return true;
                case AccountCases.TRANSFER:
                    amount = helper.GetAmount();
                    long transferAccountID = GetTransferAccountID();
                    bool isTransferred = transactionController.Transfer(amount, account, transferAccountID);
                    return true;
                case AccountCases.CHECK_BALANCE:
                    Console.WriteLine($"BALANCE: {account.Balance}");
                    return false;
                case AccountCases.VIEW_STATEMENT:
                    ViewStatement(statementServies);
                    return false;
                case AccountCases.PRINT_STATEMENT:
                    PrintStatement(statementServies);
                    return false;
                case AccountCases.BACK:
                    return true;
                default:
                    Console.WriteLine("Invalid option");
                    return false;
            }
        }

        public long GetTransferAccountID()
        {
            while (true)
            {
                Console.WriteLine("Enter Account ID to transfer: ");
                try
                {
                    string id = Console.ReadLine().Trim();
                    long intID = long.Parse(id);
                    return intID;
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid ID.");
                    GetTransferAccountID();
                }
            }
        }

        public void PrintStatement(TransactionController transactionController)
        {
            IList<Transaction> statements = transactionController.GetAllTransactions();
            Printer.PrintStatement(statements);

        }
        public void ViewStatement(TransactionController transactionController)
        {
            transactionController.ViewAllTransactions();
        }

    }
}
