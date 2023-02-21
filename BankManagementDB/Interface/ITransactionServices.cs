using BankManagement.Model;
using BankManagement.Models;
using BankManagementCipher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ITransactionServices
    {

        public bool InsertTransaction(Transaction transaction);

        IEnumerable<Transaction> GetAllTransactions(Guid accountID);

        void FillTable(Guid accountID);

    }
}
