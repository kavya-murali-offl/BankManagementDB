using System;
using BankManagementDB.Properties;
using BankManagementDB.EnumerationType;
using BankManagementDB.Utility;
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
            Formatter.FormatString(Resources.DisplayTransaction, TransactionType, RecordedOn, Description, Amount, Balance, ModeOfPayment);
              
    }
}
