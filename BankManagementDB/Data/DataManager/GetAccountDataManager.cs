using BankManagementDB.Data;
using BankManagementDB.Interface;
using BankManagementDB.Models;
using System.Collections.Generic;

namespace BankManagementDB.DataManager
{
    public class GetAccountDataManager : IGetAccountDataManager
    {
        public GetAccountDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }
        private IDBHandler DBHandler { get; set; }

        public IList<Account> GetAllAccounts(string customerId)
        {
            IList<Account> accounts = DBHandler.GetAccounts(customerId).Result;
            Store.AccountsList = accounts;
            return accounts;
        }

    }
}
