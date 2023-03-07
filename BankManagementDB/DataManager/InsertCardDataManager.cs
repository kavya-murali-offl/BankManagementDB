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
    public class InsertCardDataManager : IInsertCardDataManager
    {
        public InsertCardDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        public IDBHandler DBHandler { get; private set; }

        public bool InsertCard(Card card)
        {
            if (card.Type == CardType.CREDIT)
            {
                CreditCard creditCard = new CreditCard();
                creditCard.ID = card.ID;
                if (!DBHandler.InsertCreditCard(creditCard).Result)
                    return false;
            }
            bool success = DBHandler.InsertCard(card).Result;
            return success;
        }
    }
}
