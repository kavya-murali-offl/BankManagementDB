using BankManagementDB.Interface;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class InsertCredentialsDataManager : IInsertCredentialsDataManager
    {

        public InsertCredentialsDataManager(IDBHandler dbHandler)
        {
            DBHandler = dbHandler;
        }

        private IDBHandler DBHandler { get; set; }

        public bool InsertCredentials(CustomerCredentials customerCredentials) => DBHandler.InsertCredentials(customerCredentials).Result;

    }
}
