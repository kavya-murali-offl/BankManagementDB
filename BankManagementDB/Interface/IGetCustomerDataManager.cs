using BankManagementDB.Models;

namespace BankManagementDB.Interface
{
    public interface IGetCustomerDataManager
    {
        public Customer GetCustomer(string phoneNumber);

    }
}
