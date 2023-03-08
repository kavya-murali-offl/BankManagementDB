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
           

                return DBHandler.InsertCustomer(customer).Result;
        }
    }
}
