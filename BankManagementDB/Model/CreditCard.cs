using BankManagementDB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Model
{
    public class CreditCard : Card
    {
        public CreditCard()
        {
            ID = Guid.NewGuid();
            CreditLimit = 10000;
            APR = 0.060m;
            ExpiryMonth = DateTime.Now.Month;
            ExpiryYear = DateTime.Now.Year + 7;
            CreditPoints = 100;
            Balance = 0;
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
           $"Card Type: CREDIT\n Card ID: {ID}\n Card Number: {CardNumber}\n CardHolderName: {CardHolder}\n Balance: {Balance}";

    }
}
