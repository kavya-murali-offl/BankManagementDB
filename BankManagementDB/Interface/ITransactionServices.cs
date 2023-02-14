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

        IList<Transaction> GetAllTransactions();

        void FillTable(Guid accountID);

    }
}
