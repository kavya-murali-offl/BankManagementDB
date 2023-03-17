using BankManagementDB.EnumerationType;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IAccountDataManager
    {
        bool InsertAccount(Account account);

        bool UpdateAccount(Account account);

        public void GetAllAccounts(Guid id);

        IList<Account> GetAccountsList();

        bool UpdateBalance(Account account, decimal amount, TransactionType transactionType);

        Account GetAccountByQuery(string key, string value);
    }
}
