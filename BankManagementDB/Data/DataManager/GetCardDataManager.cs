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

        private IDBHandler DBHandler { get;  set; }

        public void GetAllCards(string customerID)
        {
            IEnumerable <Card> cards = new List<Card>();
            var creditCardsList = DBHandler.GetCreditCardByCustomerID(customerID).Result;
            var debitCardsList = DBHandler.GetDebitCardByCustomerID(customerID).Result;
            cards = cards.Concat(creditCardsList);
            cards = cards.Concat(debitCardsList);
            Store.CardsList = cards;
        }

    }
}
