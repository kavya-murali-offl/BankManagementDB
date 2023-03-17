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

        public bool InsertCustomer(Customer customer, string password) => DBHandler.InsertCustomer(customer).Result;
    }
}
