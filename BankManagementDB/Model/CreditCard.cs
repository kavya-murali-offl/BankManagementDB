using BankManagementDB.EnumerationType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Model
{
    public class CreditCard : Card
    {
        public CreditCard() : base()
        {
            CreditLimit = 10000;
            APR = 0.060m;
            CreditPoints = 100;
            Type = CardType.CREDIT;
        }

        public override void Purchase(decimal amount)
        {
            Balance += amount;
        }

        public override void Payment(decimal amount)
        {
            Balance -= amount;
        }



        public override string ToString() =>
           $"\nCard Type: CREDIT\nCard Number: {CardNumber}\nBalance: {Balance}\nExpiry Month: {ExpiryMonth}\nExpiry Year: {ExpiryYear}\n=====================================================\n";

    }
}
