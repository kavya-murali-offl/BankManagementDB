using BankManagementCipher.Model;
using BankManagementDB.Interface;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class GetCustomerPasswordDataManager
    {
        public GetCustomerPasswordDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        public IDBHandler DBHandler { get; set; }

        public string GetPassword(Guid id) => DBHandler.GetCredentials(id).Result.FirstOrDefault().Password;
    }
}
