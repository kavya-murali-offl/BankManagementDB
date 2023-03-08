using BankManagementDB.Interface;
using BankManagementDB.Models;
using System.Linq;

namespace BankManagementDB.DataManager
{
    public class GetCustomerDataManager  : IGetCustomerDataManager
    {
        public GetCustomerDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }
        public IDBHandler DBHandler { get; set; }
        
        public Customer GetCustomer(string phoneNumber) => DBHandler.GetCustomer(phoneNumber).Result.FirstOrDefault();
    }
}
