using BankManagementDB.Entities;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Data.DataManager
{
    public class InsertSavingsAccountDataManager : IInsertSavingsAccountDataManager
    {
        public InsertSavingsAccountDataManager(IDBHandler dbHandler)
        {
            DBHandler = dbHandler;
        }

        private IDBHandler DBHandler { get; set; }

        public bool InsertSavingsAccount(SavingsAccountDTO account) => DBHandler.InsertSavingsAccount(account).Result;
    }
}
