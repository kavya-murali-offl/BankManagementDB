using BankManagementDB.Controller;
using BankManagementDB.Data;
using BankManagementDB.Interface;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class GetAccountDataManager : IGetAccountDataManager
    {
        public GetAccountDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }
        public IDBHandler DBHandler { get; private set; }

        public IList<Account> GetAllAccounts(string customerId)
        {
            IList<Account> accounts = DBHandler.GetAccounts(customerId).Result;
            Store.AccountsList = accounts;
            return accounts;
        }

    }
}
