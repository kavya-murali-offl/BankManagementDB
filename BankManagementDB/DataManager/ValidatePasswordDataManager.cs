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
    public class ValidatePasswordDataManager : IValidatePasswordDataManager
    {
        public ValidatePasswordDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        public IDBHandler DBHandler { get; private set; }

        public bool ValidatePassword(Guid customerID, string password)
        {
            string hashedInput = AuthServices.Encrypt(password);
            CustomerCredentials customerCredentials = DBHandler.GetCredentials(customerID).Result.FirstOrDefault();
            if (customerCredentials != null)
            {
                return hashedInput == customerCredentials.Password ? true : false;
            }
            return false;
        }
    }
}
