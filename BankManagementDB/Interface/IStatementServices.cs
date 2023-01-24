using BankManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IStatementServices
    {
        IList<Transaction> GetAllTransactions();
        void ViewAllTransactions();

    }
}
