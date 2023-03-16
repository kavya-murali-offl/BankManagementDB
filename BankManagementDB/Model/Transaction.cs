using System;
using BankManagementDB.EnumerationType;
using SQLite;

namespace BankManagementDB.Model
{
    [Table("Transactions")]
    public class Transaction
    {
        [PrimaryKey]
        public string ID { get; set; }

        public TransactionType TransactionType { get; set; }

        public ModeOfPayment ModeOfPayment { get; set; }

        public decimal Amount { get; set; }

        public DateTime RecordedOn { get; set; }

        public decimal Balance { get; set; }

        public string FromAccountNumber { get; set; }

        public string ToAccountNumber { get; set; }

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
