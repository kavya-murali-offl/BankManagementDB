using BankManagementDB.Interface;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class UpdateCreditCardDataManager : IUpdateCreditCardDataManager
    {
        public UpdateCreditCardDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        private IDBHandler DBHandler { get; set; }

        public bool UpdateCreditCard(CreditCardDTO updatedCard) => DBHandler.UpdateCreditCard(updatedCard).Result;
    }
}
