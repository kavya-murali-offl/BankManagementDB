using BankManagementDB.Controller;
using BankManagementDB.Data;
using BankManagementDB.DBHandler;
using BankManagementDB.Interface;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class InsertAccountDataManager : IInsertAccountDataManager
    {
        public InsertAccountDataManager(IDBHandler dbHandler)
        {
            DBHandler = dbHandler;
        }

        public IDBHandler DBHandler { get; private set; }

        public bool InsertAccount(Account account)
        {
            bool isSuccess = DBHandler.InsertAccount(account).Result;
            if (isSuccess)
            {
                CacheData.AccountsList ??= new List<Account>();
                CacheData.AccountsList.Insert(0, account);
            }
            return isSuccess;
        }
    }
}
