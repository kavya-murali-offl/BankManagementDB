using BankManagement.Models;

namespace BankManagementDB.Interface
{
    public interface ICustomerServices
    {
        Customer GetCustomer(string phone);

        bool InsertCustomer(Customer customer, string password);

        Customer UpdateCustomer(Customer customer);

    }
}
