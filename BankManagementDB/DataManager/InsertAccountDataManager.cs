using BankManagementCipher.Model;
using BankManagementDB.Controller;
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
        public InsertAccountDataManager(IDBHandler dbHandler, IObjectMappingDataManager objectMappingDataManager)
        {
            ObjectMappingDataManager = objectMappingDataManager;
            DBHandler = dbHandler;
        }
        public IDBHandler DBHandler { get; private set; }

        public IObjectMappingDataManager ObjectMappingDataManager { get; private set; }


        public bool InsertAccount(Account account)
        {
            return DBHandler.InsertAccount(ObjectMappingDataManager.AccountToDto(account)).Result;
          
        }
    }
}
