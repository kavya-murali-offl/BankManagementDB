

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
        DEPOSIT, WITHDRAW, TRANSFER, VIEW_STATEMENT, PRINT_STATEMENT, BACK
    }

    public class TransactionsView
    {
        public void GoToAccount(Account account, AccountsController accountsController)
        {
            while (true)
            {
                Console.WriteLine("\n1. Deposit \n2. Withdraw\n3. Transfer\n4. View Statement\n5. Print Statement\n6. Back \nEnter your choice: ");
                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(AccountCases)).Count())
                    {
                        AccountCases operation = (AccountCases)entryOption - 1;
                        if (TransactionOperations(operation, account, accountsController))
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

        public bool TransactionOperations(AccountCases option, Account account, AccountsController accountsController)
        {
            Helper helper = new Helper();
            ITransactionServices transactionController = new TransactionController();
            decimal amount;
            switch (option)
            {
                case AccountCases.DEPOSIT:
                    amount = helper.GetAmount();
                    bool isDeposited = transactionController.Deposit(amount, account);
                    if (isDeposited) Console.WriteLine("Deposit succesful");
                    else Console.WriteLine("Something went wrong.");
                    return true;
                case AccountCases.WITHDRAW:
                    amount = helper.GetAmount();
                    bool isWithdrawn = transactionController.Withdraw(amount, account);
                    if (isWithdrawn) Console.WriteLine("Withdraw succesful");
                    else Console.WriteLine("Something went wrong.");
                    return true;
                case AccountCases.TRANSFER:
                    amount = helper.GetAmount();
                    long transferAccountID = GetTransferAccountID();
                    bool isTransferred = transactionController.Transfer(amount, account, transferAccountID);
                    if (isTransferred) Console.WriteLine("Transfer succesful");
                    else Console.WriteLine("Something went wrong.");
                    return true;
                case AccountCases.VIEW_STATEMENT:
                    //ViewStatement();
                    return false;
                case AccountCases.PRINT_STATEMENT:
                    //PrintStatement();
                    return false;
                case AccountCases.BACK:
                    return true;
                default:
                    Console.WriteLine("Invalid option");
                    return false;
            }
        }

        public Int64 GetTransferAccountID()
        {
            while (true)
            {
                Console.WriteLine("Enter Account ID to transfer: ");
                try
                {
                    string id = Console.ReadLine().Trim();
                    Int64 intID = Int64.Parse(id);
                    return intID;
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid ID.");
                }
            }
        }

        //public void PrintStatement()
        //{

        //    IList<Transaction> statements = transactionController.GetAllTransactions();
        //    Printer.PrintStatement(statements);

        //}
        //public void ViewStatement(TransactionController transactionController)
        //{
        //    transactionController.ViewAllTransactions();
        //}

        //    public void PrintStatement()
        //    {
        //        IList<Transaction> transactions = transactionController.GetAllTransactions();
        //        Printer.PrintStatement(transactions);
        //    }
        //}
    }
}
