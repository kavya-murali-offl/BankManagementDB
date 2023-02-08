using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.db
{
    public class DatabaseContext : DataContext
    {
        public List<Customer> Customers;

        public DatabaseContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}
