using BankManagementDB.Interface;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class InsertCreditCardDataManager : IInsertCreditCardDataManager
    {
        public InsertCreditCardDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        public IDBHandler DBHandler { get; private set; }

        public bool InsertCreditCard(CreditCard card) => DBHandler.InsertCreditCard(card).Result;

    }
}
