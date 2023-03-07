using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class GetCardDataManager : IGetCardDataManager
    {
        public GetCardDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        public static IList<CardBObj> CardsList { get; set; }

        public IDBHandler DBHandler { get; private set; }

        public void GetAllCards(Guid customerID)
        {
            var cardsList = DBHandler.GetCard(customerID).Result;
            IList<CardBObj> cards = new List<CardBObj>();
            if (cardsList.Count() > 0)
                foreach (var card in cardsList)
                    cards.Add(card);
            CardsList = cards;
        }

        public bool IsDebitCardEnabled(Guid accountID) => CardsList.Where(c => c.Type.Equals(CardType.DEBIT) && c.AccountID.Equals(accountID)).Count() > 0;

        public bool IsCreditCardEnabled() => CardsList.Where(c => c.Type.Equals(CardType.CREDIT)).Count() > 0;

        public CardBObj GetCardByType(CardType cardType) => CardsList.Where<CardBObj>(card => card.Type == cardType).FirstOrDefault();

        public bool IsCardNumber(string cardNumber) => CardsList.Where<CardBObj>(card => card.CardNumber == cardNumber).Any();

        public CardBObj GetCard(string cardNumber) => CardsList.Where<CardBObj>(card => card.CardNumber == cardNumber).FirstOrDefault();

        public IList<CardBObj> GetCardsList() => CardsList ??= new List<CardBObj>();

        public bool IsCreditCard(string cardNumber) => CardsList.Where(c => c.Type == CardType.CREDIT && c.CardNumber == cardNumber).Any();

        public bool IsDebitCardLinked(Guid accountID) => CardsList.Where(card => card.AccountID == accountID).Count() > 0;

        public bool Authenticate(string cardNumber, string pin)
        {
            CardBObj card = GetCard(cardNumber);
            if (card != null)
                return card.Pin == pin;
            return false;
        }
    }
}
