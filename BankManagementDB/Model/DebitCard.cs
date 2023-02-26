using BankManagementDB.EnumerationType;

namespace BankManagementDB.Model
{
    public class DebitCard : Card
    {
        public DebitCard() : base() {
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
          $"\nCard Type: DEBIT\nCard Number: {CardNumber}\nBalance: {Balance}\n =======================================================\n";
    }
}
