using BankManagementDB.EnumerationType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Model
{
    public class CardBObj
    {
        public Guid ID { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Pin { get; set; }

        public string CardNumber { get; set; }

        public Guid AccountID { get; set; }

        public Guid CustomerID { get; set; }

        public string CVV { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public CardType Type { get; set; }

        public CreditCardType CreditCardType { get; set; }

        public int CreditPoints { get; set; }

        public decimal TotalDueAmount { get; set; }

        public decimal APR { get; set; }

        public decimal CreditLimit { get; set; }

        public override string ToString() =>
         $"\nCard Type: {Type.ToString()}\nCard Number: {CardNumber}\nExpiry Month: {ExpiryMonth}\nExpiry Year: {ExpiryYear}\n=====================================================\n";
    }
}
