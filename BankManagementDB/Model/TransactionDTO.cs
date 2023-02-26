using BankManagementDB.EnumerationType;
using SQLite;
using System;

namespace BankManagementCipher.Model
{
    [Table("Transactions")]
    public class TransactionDTO
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public string TransactionType { get; set; }

        public string ModeOfPayment { get; set; }

        public decimal Amount { get; set; }

        public DateTime RecordedOn { get; set; }

        public decimal Balance { get; set; }

        public string Description { get; set; }

        public Guid AccountID { get; set; }
    }
}
