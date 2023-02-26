
using BankManagementDB.EnumerationType;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ICardController
    {
        bool InsertCard(Card card);

        bool UpdateCard(Card card);

        Card GetCardByType(CardType cardType);

        void GetAllCards(Guid customerID);

        Card CreateCard(CardType cardType, Guid accountID, Guid customerID);

        IList<Card> GetCardsList();

        Card GetCard(string cardNumber);

        bool IsCreditCardEnabled();

        bool IsDebitCardEnabled();

        bool IsCardNumber(string cardNumber);

        bool ResetPin(string cardNumber, string pin);

    }
}
