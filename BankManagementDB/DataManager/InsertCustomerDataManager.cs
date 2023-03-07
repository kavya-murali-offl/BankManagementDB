using BankManagementCipher.Model;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;

namespace BankManagementDB.DataManager
{
    public class InsertCustomerDataManager : IInsertCustomerDataManager
    {
        public InsertCustomerDataManager(IDBHandler dbHandler)
        {
            DBHandler = dbHandler;
        }

        public IDBHandler DBHandler { get; set; }

        public bool InsertCustomer(Customer customer, string password)
        {
            string hashedPassword = AuthServices.Encrypt(password);
            CustomerCredentials customerCredentials = new CustomerCredentials();
            customerCredentials.Password = hashedPassword;
            customerCredentials.ID = customer.ID;
            if (DBHandler.InsertCredentials(customerCredentials).Result)
                return DBHandler.InsertCustomer(customer).Result;
            return false;
        }
    }
}
