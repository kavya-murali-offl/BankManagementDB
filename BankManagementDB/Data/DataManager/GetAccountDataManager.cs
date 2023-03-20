using BankManagementDB.Data;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankManagementDB.DataManager
{
    public class GetAccountDataManager : IGetAccountDataManager
    {
        public GetAccountDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }
        private IDBHandler DBHandler { get; set; }

        public void GetAllAccounts(string customerId)
        {
            IList<SavingsAccount> savingsAccounts = DBHandler.GetSavingsAccounts(customerId).Result;
            IList<CurrentAccount> currentAccounts = DBHandler.GetCurrentAccounts(customerId).Result;
            Store.AccountsList = new List<Account>();
            Store.AccountsList = Store.AccountsList.Concat(currentAccounts);
            Store.AccountsList = Store.AccountsList.Concat(savingsAccounts);
        }

    }
}
