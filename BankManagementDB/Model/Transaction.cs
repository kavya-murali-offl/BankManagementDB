using System;
using BankManagementDB.EnumerationType;
using SQLite;

namespace BankManagementDB.Model
{
    [Table("Transactions")]
    public class Transaction
    {

        public Transaction()
        {
            ID = Guid.NewGuid();
            RecordedOn = DateTime.Now;
        }

        public Transaction(string description, decimal amount, decimal balance, TransactionType transactionType, Guid accountID, ModeOfPayment modeOfPayment, string cardNumber)
        {
            ID = Guid.NewGuid();
            Description = description;
            TransactionType = transactionType;
            Amount = amount;
            Balance = balance;  
            RecordedOn = DateTime.Now;
            AccountID = accountID;
            ModeOfPayment = modeOfPayment;
            CardNumber = cardNumber;
        }

        [PrimaryKey]
        public Guid ID { get; set; }

        public TransactionType TransactionType { get; set; }

        public ModeOfPayment ModeOfPayment { get; set; }

        public decimal Amount { get; set; }

        public DateTime RecordedOn { get; set; }

        public decimal Balance { get; set; }

        public Guid AccountID { get; set; }

        public string Description { get; set; }

        public string CardNumber { get; set; }

        public override string ToString() =>
                $@"
                Transaction Type: {TransactionType} 
                Transaction Time: {RecordedOn} 
                Description: {Description}
                Amount: {Amount}
                Mode of Payment: {ModeOfPayment}
                Balance: {Balance}";
    }
}
