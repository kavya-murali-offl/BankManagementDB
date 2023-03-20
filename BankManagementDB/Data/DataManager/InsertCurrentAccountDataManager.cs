using BankManagementDB.Entities;
using BankManagementDB.Interface;

namespace BankManagementDB.Data.DataManager
{
    public class InsertCurrentAccountDataManager : IInsertCurrentAccountDataManager
    {
        public InsertCurrentAccountDataManager(IDBHandler dbHandler)
        {
            DBHandler = dbHandler;
        }

        private IDBHandler DBHandler { get; set; }

        public bool InsertCurrentAccount(CurrentAccountDTO account) => DBHandler.InsertCurrentAccount(account).Result;  
    }
}
