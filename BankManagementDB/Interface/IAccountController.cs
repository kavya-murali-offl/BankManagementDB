using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IAccountController
    {

        bool InsertAccount(Account account);

        bool UpdateAccount(Account account);

        public void GetAllAccounts(Guid id);

        IList<Account> GetAccountsList();

        Account GetAccountByQuery(string key, object value);
    }
}
