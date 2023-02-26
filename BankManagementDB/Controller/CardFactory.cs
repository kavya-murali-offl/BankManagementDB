
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;

namespace BankManagementDB.Controller
{
    public class CardFactory : ICardFactory
    {
        public Card GetCardByType(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.CREDIT:
                    {
                        CreditCard creditCard = new CreditCard();
                        return creditCard;
                    }
                case CardType.DEBIT:
                    {
                        DebitCard debitCard = new DebitCard();
                        return debitCard;
                    }
                default:
                    return null;
            }
        }
    }
}
