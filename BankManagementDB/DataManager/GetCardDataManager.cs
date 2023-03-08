using BankManagementDB.Data;
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

        public IDBHandler DBHandler { get; private set; }

        public void GetAllCards(Guid customerID)
        {
            var cardsList = DBHandler.GetCard(customerID).Result;
            IList<CardBObj> cards = new List<CardBObj>();
            foreach (var card in cardsList)
                cards.Add(card);
            CacheData.CardsList = cards;
        }

        public bool IsDebitCardEnabled(Guid accountID) => CacheData.CardsList.Where(c => c.Type.Equals(CardType.DEBIT) && c.AccountID.Equals(accountID)).Any();

        public bool IsCreditCardEnabled() => CacheData.CardsList.Where(c => c.Type.Equals(CardType.CREDIT)).Any();

        public CardBObj GetCardByType(CardType cardType) => CacheData.CardsList.Where<CardBObj>(card => card.Type == cardType).FirstOrDefault();

        public bool IsCardNumber(string cardNumber) => CacheData.CardsList.Where<CardBObj>(card => card.CardNumber == cardNumber).Any();

        public CardBObj GetCard(string cardNumber) => CacheData.CardsList.Where<CardBObj>(card => card.CardNumber == cardNumber).FirstOrDefault();

        public IList<CardBObj> GetCardsList() => CacheData.CardsList ??= new List<CardBObj>();

        public bool IsCreditCard(string cardNumber) => CacheData.CardsList.Where(c => c.Type == CardType.CREDIT && c.CardNumber == cardNumber).Any();

        public bool IsDebitCardLinked(Guid accountID) => CacheData.CardsList.Where(card => card.AccountID == accountID).Any();

    }
}
