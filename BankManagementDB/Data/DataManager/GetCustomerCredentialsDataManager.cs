using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class GetCustomerCredentialsDataManager : IGetCustomerCredentialsDataManager
    {
        public GetCustomerCredentialsDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }
        private IDBHandler DBHandler { get; set; }

        public CustomerCredentials GetCustomerCredentials(string id) => DBHandler.GetCredentials(id).Result.FirstOrDefault();

    }
}
