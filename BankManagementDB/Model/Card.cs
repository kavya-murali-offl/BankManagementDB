using BankManagementDB.EnumerationType;
using System;

namespace BankManagementDB.Model
{
    public abstract class Card
    {
        public Card()
        {
            ID = Guid.NewGuid();
            ExpiryMonth = DateTime.Now.Month.ToString();
            ExpiryYear = (DateTime.Now.Year + 7).ToString();
            CreatedOn = DateTime.Now;
            Balance = 0;
        }

        public Guid ID { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreditPoints { get; set; }

        public string Pin { get; set; }

        public string CardNumber { get; set; }

        public Guid AccountID { get; set; }

        public CreditCardType CreditCardType { get; set; }

        public Guid CustomerID { get; set; }

        public decimal Balance { get; set; }

        public string CVV { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public decimal APR { get; set; }

        public decimal CreditLimit { get; set; }

        public CardType Type { get; set; }

        public abstract void Purchase(decimal amount);

        public abstract void Payment(decimal amount);

        public abstract override string ToString();

    }
}
