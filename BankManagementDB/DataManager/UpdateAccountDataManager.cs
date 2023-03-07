using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public  class UpdateAccountDataManager : IUpdateAccountDataManager
    {
        public UpdateAccountDataManager(IDBHandler dBHandler, IObjectMappingDataManager objectMappingDataManager)
        {
            DBHandler = dBHandler;
            ObjectMappingDataManager = objectMappingDataManager;
        }
        public IDBHandler DBHandler { get; private set; }

        public IObjectMappingDataManager ObjectMappingDataManager { get; private set; }

        public bool UpdateBalance(Account account, decimal amount, TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.DEPOSIT:
                    account.Deposit(amount);
                    return UpdateAccount(account);
                case TransactionType.WITHDRAW:
                    account.Withdraw(amount);
                    return UpdateAccount(account);
                default:
                    break;
            }
            return false;
        }

        public bool UpdateAccount(Account updatedAccount) => DBHandler.UpdateAccount(ObjectMappingDataManager.AccountToDto(updatedAccount)).Result;

    }
}
