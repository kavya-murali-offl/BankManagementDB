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

        private IDBHandler DBHandler { get; set; }

        public bool InsertCustomer(Customer customer, CustomerCredentials customerCredentials){
            if (DBHandler.InsertCredentials(customerCredentials).Result)
            {
                return DBHandler.InsertCustomer(customer).Result;
            }
            return false;
       }
    }
}
