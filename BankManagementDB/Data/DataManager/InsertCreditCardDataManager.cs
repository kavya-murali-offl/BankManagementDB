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

        private IDBHandler DBHandler { get; set; }

        public bool InsertCreditCard(CreditCardDTO card) => DBHandler.InsertCreditCard(card).Result;

    }
}
