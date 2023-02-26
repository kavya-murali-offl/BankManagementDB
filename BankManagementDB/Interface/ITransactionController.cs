using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ITransactionController
    {
        public bool InsertTransaction(Transaction transaction);

        IEnumerable<Transaction> GetAllTransactions(Guid accountID);

        void FillTable(Guid accountID);

        public DateTime? GetLastWithdrawDate();
    }
}
