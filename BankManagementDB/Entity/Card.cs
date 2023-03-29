using BankManagementDB.Properties;
using BankManagementDB.EnumerationType;
using BankManagementDB.Utility;
using SQLite;
using System;

namespace BankManagementDB.Model
{
    [Table("Card")]
    public class Card
    {
        [PrimaryKey]
        public string ID { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Pin { get; set; }

        public string CardNumber { get; set; }

        public string AccountID { get; set; }

        public string CustomerID { get; set; }

        public string CVV { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public CardType Type { get; set; }

        public override string ToString() =>
            Formatter.FormatString(Resources.DisplayCard, Type.ToString(), CardNumber, ExpiryMonth, ExpiryYear);
    }
}
