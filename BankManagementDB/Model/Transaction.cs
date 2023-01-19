using System;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagementDB.Interface;

namespace BankManagement.Model
{

    public class Transaction
    {
        public Transaction()
        {
            RecordedOn = DateTime.Now;
        }

        public Transaction(string description, decimal amount, decimal balance, TransactionTypes transactionType)
        {
            Description = description;
            TransactionType = transactionType;
            Amount = amount;
            Balance = balance;  
            RecordedOn = DateTime.Now;
        }

        public TransactionTypes TransactionType { get; set; }

        public decimal Amount { get; set; }

        public DateTime RecordedOn { get; set; }

        public decimal Balance { get; set; }

        public long AccountID { get; set; }

        public long ID { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return $@"Transaction ID: {ID}
                \tTransaction Type: {TransactionType} 
                \tTransaction Time: {RecordedOn} 
                \tDescription: {Amount}
                \tAmount: {Amount}
                \tBalance: {Balance}";
        }
    }
}
