using BankManagementDB.Controller;
using BankManagementDB.Data;
using BankManagementDB.DatabaseHandler;
using BankManagementDB.Entities;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BankManagementDB.DataManager
{
    public class InsertAccountDataManager : IInsertAccountDataManager
    {
        public InsertAccountDataManager(IDBHandler dbHandler)
        {
            DBHandler = dbHandler;
        }

        private IDBHandler DBHandler { get; set; }

        public bool InsertAccount(Account account)
        {

            bool isSuccess = DBHandler.InsertAccount(account).Result;
            Store.AccountsList ??= new List<Account>();

            if (isSuccess)
            {
                if (account is CurrentAccount)
                {
                    CurrentAccountDTO currentAccount = new CurrentAccountDTO();
                    currentAccount.ID = account.ID;
                    DBHandler.InsertCurrentAccount(currentAccount);
                }
                else if (account is SavingsAccount)
                {
                    SavingsAccountDTO savingsAccountDTO = new SavingsAccountDTO();
                    savingsAccountDTO.ID = account.ID;
                    DBHandler.InsertSavingsAccount(savingsAccountDTO);
                }
                Store.AccountsList.Prepend(account);
            }
            return isSuccess;
        }
    }
}
