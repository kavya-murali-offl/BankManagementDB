using BankManagementDB.Models;
using BankManagementCipher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ICustomerRepository
    {

        Task<bool> InsertOrReplace(CustomerDTO customer);

        Task<IList<CustomerDTO>> Get(Guid id);

        Task<IList<CustomerDTO>> Get(string phoneNUmber);

        string GetHashedPassword(string phoneNumber);
    }
}
