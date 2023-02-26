using System;
using BankManagementDB.EnumerationType;
using SQLite;

namespace BankManagementDB.Model
{
    [Table("Card")]
    public class CardDTO
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public string Pin { get; set; }

        [Unique]
        public string CardNumber { get; set; }

        public Guid AccountID { get; set; }

        public Guid CustomerID { get; set; }

        public string CreditCardType { get; set; }

        public decimal Balance { get; set; }

        public string CVV { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public decimal APR { get; set; }

        public decimal CreditLimit { get; set; }

        public int CreditPoints { get; set; }

        public string Type { get; set; }
    }
}
