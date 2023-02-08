using System;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagementDB.Interface;
using System.Data.Linq.Mapping;

namespace BankManagement.Model
{
    [Table(Name = "Transactions")]
    public class Transaction
    {
        public Transaction()
        {
            ID = Guid.NewGuid();
            RecordedOn = DateTime.Now;
        }

        public Transaction(string description, decimal amount, decimal balance, TransactionTypes transactionType)
        {
            ID = Guid.NewGuid();
            Description = description;
            TransactionType = transactionType;
            Amount = amount;
            Balance = balance;  
            RecordedOn = DateTime.Now;
        }
        [Column]
        public Guid ID { get; set; }
        [Column]

        public TransactionTypes TransactionType { get; set; }
        [Column]

        public decimal Amount { get; set; }
        [Column]

        public DateTime RecordedOn { get; set; }
        [Column]

        public decimal Balance { get; set; }
        [Column]

        public Guid AccountID { get; set; }
        [Column]

        public string Description { get; set; }

        public override string ToString() =>
             $@"
                Transaction Type: {TransactionType} 
                Transaction Time: {RecordedOn} 
                Description: {Amount}
                Amount: {Amount}
                Balance: {Balance}";
    }
}
