using BankManagementDB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Model
{
    public class DebitCard : Card
    {
        public DebitCard() {
            ID = Guid.NewGuid();
            ExpiryMonth = DateTime.Now.Month;
            ExpiryYear = DateTime.Now.Year + 7;
            CreditPoints = 0;
            Balance = 0;
            Type = CardType.DEBIT;
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
          $" Card Type: DEBIT\n Card ID: {ID}\n Card Number: {CardNumber}\n CardHolderName: {CardHolder}\n Balance: {Balance}";
    }
}
