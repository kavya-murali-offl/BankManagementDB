using System;
using BankManagement.Enums;
using BankManagement.Models;

namespace BankManagement.Model
{
    public class Transaction
    {
        private static int TransactionID = 1;
        private DateTime _recordedOn;

        public Transaction(decimal amount, decimal balance, TransactionTypes transactionType)
        {
            TransactionID = TransactionID + 1;
            TransactionType = transactionType;
            Amount = amount;
            _recordedOn = DateTime.Now;
        }

        public TransactionTypes TransactionType { get; set; }

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public Account Account { get; set; } 

        public override string ToString()
        {
            return "Transaction ID: " +
                "Transaction Type: " + TransactionType + "\nTransaction ID: "+TransactionID + "\nTransaction Time: " +
                _recordedOn + "\nAmount: " + Amount +
                "\nBalance: " + Balance;
        }

    }

}
