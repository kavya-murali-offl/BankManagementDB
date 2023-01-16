using System;
using BankManagement.Enums;
using BankManagement.Models;

namespace BankManagement.Model
{
    public class Transaction
    {
        public Transaction()
        {
            RecordedOn = DateTime.Now;
        }
        public Transaction(decimal amount, decimal balance, TransactionTypes transactionType)
        {
            TransactionType = transactionType;
            Amount = amount;
            Balance = balance;  
            RecordedOn = DateTime.Now;
        }

        public TransactionTypes TransactionType { get; set; }

        public decimal Amount { get; set; }

        public DateTime RecordedOn { get; set; }

        public decimal Balance { get; set; }

        public Int64 AccountID { get; set; }

        public Int64 ID { get; set; }

        public override string ToString()
        {
            return "Transaction ID: " +
                "Transaction Type: " + TransactionType + "\nTransaction ID: "+ID + "\nTransaction Time: " +
                RecordedOn + "\nAmount: " + Amount +
                "\nBalance: " + Balance;
        }
    }
}
