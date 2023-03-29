using BankManagementDB.Properties;
using BankManagementDB.EnumerationType;
using BankManagementDB.Utility;

namespace BankManagementDB.Model
{
    public class CreditCard : Card
    {
     
        public CreditCardType CreditCardType { get; set; }

        public int CreditPoints { get; set; }

        public decimal TotalDueAmount { get; set; }

        public decimal APR { get; set; }

        public decimal CreditLimit { get; set; }

        public override string ToString() =>
            Formatter.FormatString(Resources.DisplayCreditCard, CreditCardType, TotalDueAmount, CreditLimit, CreditPoints);

    }

}
