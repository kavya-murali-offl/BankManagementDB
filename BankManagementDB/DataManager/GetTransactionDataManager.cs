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

        public IDBHandler DBHandler { get; private set; }

        public IEnumerable<Transaction> GetAllTransactions(Guid accountID)
        {
            CacheData.TransactionList = DBHandler.GetTransaction(accountID).Result;
            return CacheData.TransactionList;
        }

        public IList<Transaction> GetTransactionsByCardNumber(string cardNumber) => DBHandler.GetTransaction(cardNumber).Result;
    }
}
