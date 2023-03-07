using BankManagementDB.Controller;
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
        public GetAccountDataManager(IDBHandler dBHandler, IObjectMappingDataManager objectMappingDataManager)
        {
            DBHandler = dBHandler;
            ObjectMappingDataManager = objectMappingDataManager;
        }
        public IDBHandler DBHandler { get; private set; }

        public IObjectMappingDataManager ObjectMappingDataManager { get; private set; }

        public static IList<Account> AccountsList { get; set; }

        public IList<Account> GetAllAccounts(Guid id)
        {
            IList<Account> AccountsList = new List<Account>();
            var accountDTOs = DBHandler.GetAccounts(id).Result;

            foreach (var accountDTO in accountDTOs)
                AccountsList.Add(ObjectMappingDataManager.DtoToAccount(accountDTO));

            return AccountsList;
        }

        public Account GetAccount(string accountNumber)
        {
            var list = from acc in GetAllAccounts(CurrentUserDataManager.CurrentUser.ID) where acc.AccountNumber == accountNumber select acc;
            return list.FirstOrDefault();
        }

        public IList<Account> GetAccountsList() => AccountsList ?? new List<Account>();
    }
}
