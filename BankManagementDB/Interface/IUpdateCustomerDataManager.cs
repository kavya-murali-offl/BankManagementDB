using BankManagementDB.Models;

namespace BankManagementDB.Interface
{
    public interface IUpdateCustomerDataManager
    {
        bool UpdateCustomer(Customer customer);

    }
}
