using System;
using System.Collections.Generic;
using System.Linq;
using BankManagementDB.Model;
using BankManagementDB.Utility;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Data;

namespace BankManagementDB.View
{
    public class TransactionView
    {

        public AccountView AccountView { get; set; }

        public void LoadAllTransactions(string accountNumber)
        {
            IGetTransactionDataManager getTransactionDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetTransactionDataManager>();
            getTransactionDataManager.GetTransactionsByAccountNumber(accountNumber);
        }

        public bool RecordTransaction(string description, decimal amount, decimal balance, TransactionType transactionType, string fromAccountNumber, ModeOfPayment modeOfPayment, string cardNumber, string toAccountNumber)
        {
            Transaction transaction = new Transaction()
            {
                ID = Guid.NewGuid().ToString(),
                Description = description,
                Amount = amount,
                Balance = balance,
                TransactionType = transactionType,
                FromAccountNumber = fromAccountNumber,
                ModeOfPayment = modeOfPayment,
                CardNumber = cardNumber,
                ToAccountNumber = toAccountNumber,
                RecordedOn = DateTime.Now,
            };

            IInsertTransactionDataManager InsertTransactionDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertTransactionDataManager>();
            return InsertTransactionDataManager.InsertTransaction(transaction);
        }

        public bool ViewAllTransactions()
        {
            IEnumerable<Transaction> statements = Store.TransactionsList;
            foreach (Transaction transaction in statements)
            {
                Console.WriteLine(transaction);
            }

            return false;
        }

        public bool PrintStatement()
        {
            IEnumerable<Transaction> statements = Store.TransactionsList;
            Printer.PrintStatement(statements);
            return false;
        }

    }
}
