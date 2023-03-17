using BankManagementDB.Data;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class GetTransactionDataManager : IGetTransactionDataManager
    {
        public GetTransactionDataManager(IDBHandler dbHandler)
        {
            DBHandler = dbHandler;
        }

        private IDBHandler DBHandler { get;  set; }

        public IEnumerable<Transaction> GetTransactionsByAccountNumber(string accountID)
        {
            Store.TransactionsList = DBHandler.GetTransactionByAccountNumber(accountID).Result;
            return Store.TransactionsList;
        }

        public IList<Transaction> GetTransactionsByCardNumber(string cardNumber) => DBHandler.GetTransactionByCardNumber(cardNumber).Result;
    }
}
