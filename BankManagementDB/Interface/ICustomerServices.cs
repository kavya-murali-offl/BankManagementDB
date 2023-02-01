using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ICustomerServices
    {
        void FillTable();

        bool InsertCustomer(Customer customer, string password);

        Customer UpdateCustomer(IDictionary<string, dynamic> parameters);

        DataRow GetUserByQuery(string query);

    }
}
